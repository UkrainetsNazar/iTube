import { apiClient } from './client';
import type { ReactionType, VideoDto, Visibility } from '@/types';

/**
 * Normalized shape every list function below returns, regardless of what
 * the backend actually sent. Endpoints that reply with a bare array
 * (`/feed`, `/subscriptions`, `/channels/{id}/videos`) don't carry a
 * `totalCount`, so it's left undefined -- the Pagination component falls
 * back to a "does this page look full" heuristic in that case instead of
 * a real page count. See src/components/common/Pagination.tsx.
 */
export interface VideoListResult {
  items: VideoDto[];
  page: number;
  pageSize: number;
  totalCount?: number;
}

export const videosApi = {
  upload: (title: string, description: string, tags: string[]) =>
    apiClient.post<{ videoId: string }>('/videos/upload', { title, description, tags }).then((r) => r.data),

  getById: (id: string) => apiClient.get<VideoDto>(`/videos/${id}`).then((r) => r.data),

  publish: (id: string, visibility: Visibility) => apiClient.patch(`/videos/${id}/publish`, { visibility }),

  setVisibility: (id: string, visibility: Visibility) => apiClient.patch(`/videos/${id}/visibility`, { visibility }),

  remove: (id: string) => apiClient.delete(`/videos/${id}`),

  react: (id: string, type: ReactionType) => apiClient.post(`/videos/${id}/react`, { type }),

  recordView: (id: string, watchedSeconds: number) => apiClient.post(`/videos/${id}/views`, { watchedSeconds }),

  feed: (page: number, pageSize: number): Promise<VideoListResult> =>
    apiClient.get<VideoDto[]>('/videos/feed', { params: { page, pageSize } }).then((r) => ({
      items: r.data,
      page,
      pageSize,
    })),

  subscriptionsFeed: (page: number, pageSize: number): Promise<VideoListResult> =>
    apiClient.get<VideoDto[]>('/videos/subscriptions', { params: { page, pageSize } }).then((r) => ({
      items: r.data,
      page,
      pageSize,
    })),

  history: (page: number, pageSize: number): Promise<VideoListResult> =>
    apiClient
      .get<{ items: VideoDto[]; totalCount: number; page: number; pageSize: number }>('/videos/history', {
        params: { page, pageSize },
      })
      .then((r) => r.data),

  liked: (page: number, pageSize: number): Promise<VideoListResult> =>
    apiClient
      .get<{ items: VideoDto[]; totalCount: number; page: number; pageSize: number }>('/videos/liked', {
        params: { page, pageSize },
      })
      .then((r) => r.data),

  channelVideos: (channelId: string, page: number, pageSize: number): Promise<VideoListResult> =>
    apiClient.get<VideoDto[]>(`/channels/${channelId}/videos`, { params: { page, pageSize } }).then((r) => ({
      items: r.data,
      page,
      pageSize,
    })),

  myVideos: (page: number, pageSize: number): Promise<VideoListResult> =>
    apiClient
      .get<{ items: VideoDto[]; totalCount: number; page: number; pageSize: number }>('/users/me/videos', {
        params: { page, pageSize },
      })
      .then((r) => r.data),
};
