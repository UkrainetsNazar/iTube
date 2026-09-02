import { apiClient } from './client';
import type { MediaStatusResponse, MediaType, MediaUploadResponse } from '@/types';

export const mediaApi = {
  upload: (
    file: File,
    mediaType: MediaType,
    target: { videoId?: string; channelId?: string },
    onUploadProgress?: (percent: number) => void
  ) => {
    const form = new FormData();
    form.append('file', file);
    form.append('mediaType', mediaType);
    if (target.videoId) form.append('videoId', target.videoId);
    if (target.channelId) form.append('channelId', target.channelId);

    return apiClient
      .post<MediaUploadResponse>('/media/upload', form, {
        headers: { 'Content-Type': 'multipart/form-data' },
        onUploadProgress: (evt) => {
          if (onUploadProgress && evt.total) {
            onUploadProgress(Math.round((evt.loaded / evt.total) * 100));
          }
        },
      })
      .then((r) => r.data);
  },

  checkStatus: (mediaAssetId: string) =>
    apiClient.get<MediaStatusResponse>(`/media/${mediaAssetId}/status`).then((r) => r.data),
};