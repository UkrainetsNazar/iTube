import { apiClient } from './client';
import type { ChannelDto } from '@/types';

export interface UpdateChannelPayload {
  name: string;
  description?: string;
  avatarBucket?: string;
  avatarKey?: string;
  bannerBucket?: string;
  bannerKey?: string;
}

export const channelsApi = {
  getById: (id: string) => apiClient.get<ChannelDto>(`/channels/${id}`).then((r) => r.data),

  update: (id: string, payload: UpdateChannelPayload) => apiClient.patch(`/channels/${id}`, payload),
};
