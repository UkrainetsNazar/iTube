import { apiClient } from './client';
import type { CommentDto, Paged } from '@/types';

export const commentsApi = {
  list: (videoId: string, page: number, pageSize: number) =>
    apiClient
      .get<Paged<CommentDto>>(`/videos/${videoId}/comments`, { params: { page, pageSize } })
      .then((r) => r.data),

  create: (videoId: string, text: string) =>
    apiClient.post<string>(`/videos/${videoId}/comments`, { text }).then((r) => r.data),

  remove: (commentId: string) => apiClient.delete(`/comments/${commentId}`),
};
