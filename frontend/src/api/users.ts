import { apiClient } from './client';
import type { AdminUserPage, Role, UserDto } from '@/types';

export const usersApi = {
  getById: (id: string) => apiClient.get<UserDto>(`/users/${id}`).then((r) => r.data),

  // 🔒 Admin or Moderator
  browse: (channelName: string, page: number, pageSize: number) =>
    apiClient.get<AdminUserPage>('/users/users', { params: { channelName, page, pageSize } }).then((r) => r.data),

  // 🔒 Admin only
  changeRole: (id: string, role: Role) => apiClient.patch(`/users/${id}/role`, { role }),
};

export const moderationApi = {
  // 🔒 Admin or Moderator
  ban: (userId: string, reason: string, expiresAt?: string) =>
    apiClient.post(`/moderation/users/${userId}/ban`, { reason, expiresAt }),

  // 🔒 Admin or Moderator
  unban: (userId: string) => apiClient.delete(`/moderation/users/${userId}/ban`),
};
