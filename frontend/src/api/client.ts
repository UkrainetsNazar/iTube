import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { tokenStorage } from './tokenStorage';
import type { ApiError, TokenPair } from '@/types';

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5010/api';

export const apiClient = axios.create({ baseURL });

// Bare axios instance (no auth header, not subject to the interceptor
// below) used only for the refresh call itself, so refreshing never
// recurses into the 401 handler.
const refreshClient = axios.create({ baseURL });

apiClient.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Single-flight refresh: if several requests 401 at once, only one
// /auth/refresh call goes out and the rest await it.
let refreshPromise: Promise<TokenPair> | null = null;

function doRefresh(): Promise<TokenPair> {
  if (!refreshPromise) {
    const refreshToken = tokenStorage.getRefreshToken();
    refreshPromise = refreshClient
      .post<TokenPair>('/auth/refresh', { refreshToken })
      .then((res) => {
        tokenStorage.setTokens(res.data);
        return res.data;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }
  return refreshPromise;
}

interface RetriableConfig extends InternalAxiosRequestConfig {
  _retried?: boolean;
}

apiClient.interceptors.response.use(
  (res) => res,
  async (error: AxiosError<ApiError>) => {
    const original = error.config as RetriableConfig | undefined;

    if (error.response?.status === 401 && original && !original._retried && tokenStorage.hasTokens()) {
      original._retried = true;
      try {
        const { accessToken } = await doRefresh();
        original.headers = original.headers ?? {};
        original.headers.Authorization = `Bearer ${accessToken}`;
        return apiClient(original);
      } catch {
        tokenStorage.clear();
        if (window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  }
);

/** Pulls the `{ code, message }` shape out of a failed request for display in a toast. */
export function extractApiErrorMessage(error: unknown, fallback = 'Something went wrong.'): string {
  if (axios.isAxiosError<ApiError>(error)) {
    return error.response?.data?.message ?? fallback;
  }
  return fallback;
}
