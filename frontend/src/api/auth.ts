import { apiClient } from './client';
import type { CurrentUser, TokenPair } from '@/types';

export const authApi = {
  register: (email: string, password: string) => apiClient.post('/auth/register', { email, password }),

  confirmEmail: (email: string, token: string) => apiClient.post('/auth/confirm-email', { email, token }),

  login: (email: string, password: string) =>
    apiClient.post<TokenPair>('/auth/login', { email, password }).then((r) => r.data),

  logout: (refreshToken: string) => apiClient.post('/auth/logout', { refreshToken }),

  changePassword: (oldPassword: string, newPassword: string) =>
    apiClient.post('/auth/change-password', { oldPassword, newPassword }),

  forgotPassword: (email: string) => apiClient.post('/auth/forgot-password', { email }),

  resetPassword: (email: string, token: string, newPassword: string) =>
    apiClient.post('/auth/reset-password', { email, token, newPassword }),

  me: () => apiClient.get<CurrentUser>('/auth/me').then((r) => r.data),

  findByEmail: (email: string) =>
    apiClient.get<{ id: string; email: string }>('/auth/users/by-email', { params: { email } }).then((r) => r.data),
};
