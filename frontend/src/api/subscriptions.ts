import { apiClient } from './client';

export const subscriptionsApi = {
  subscribe: (targetChannelId: string) => apiClient.post('/subscriptions', { targetChannelId }),

  unsubscribe: (targetChannelId: string) =>
    apiClient.delete('/subscriptions', { data: { targetChannelId } }),

  check: (channelId: string) =>
    apiClient.get<boolean>('/subscriptions/check', { params: { channelId } }).then((r) => r.data),

  mine: () => apiClient.get<{ channelId: string }[]>('/subscriptions/me').then((r) => r.data),
};
