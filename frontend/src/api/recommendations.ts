import { apiClient } from './client';
import type { VideoDto } from '@/types';

export const recommendationsApi = {
  get: (params: { videoId?: string; limit?: number }) =>
    apiClient.get<VideoDto[]>('/recommendations', { params }).then((r) => r.data),
};
