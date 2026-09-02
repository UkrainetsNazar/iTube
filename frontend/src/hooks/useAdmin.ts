import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { authApi } from '@/api/auth';
import { moderationApi, usersApi } from '@/api/users';
import { useToast } from '@/components/common/ToastProvider';
import { extractApiErrorMessage } from '@/api/client';
import type { Role } from '@/types';

export function useAdminUserBrowse(channelName: string, page: number, pageSize = 20) {
  return useQuery({
    queryKey: ['admin', 'users', channelName, page, pageSize],
    queryFn: () => usersApi.browse(channelName, page, pageSize),
  });
}

export function useUserDetail(userId: string | undefined) {
  return useQuery({
    queryKey: ['admin', 'user-detail', userId],
    queryFn: () => usersApi.getById(userId as string),
    enabled: Boolean(userId),
  });
}

export function useFindByEmail() {
  const { showToast } = useToast();
  return useMutation({
    mutationFn: (email: string) => authApi.findByEmail(email),
    onError: (err) => showToast(extractApiErrorMessage(err, 'No user found with that email.')),
  });
}

export function useChangeRole() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  return useMutation({
    mutationFn: ({ userId, role }: { userId: string; role: Role }) => usersApi.changeRole(userId, role),
    onSuccess: (_data, variables) => {
      showToast('Role updated.', 'success');
      queryClient.invalidateQueries({ queryKey: ['admin', 'user-detail', variables.userId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not change role.')),
  });
}

export function useBanUser() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  return useMutation({
    mutationFn: ({ userId, reason, expiresAt }: { userId: string; reason: string; expiresAt?: string }) =>
      moderationApi.ban(userId, reason, expiresAt),
    onSuccess: (_data, variables) => {
      showToast('User banned.', 'success');
      queryClient.invalidateQueries({ queryKey: ['admin', 'user-detail', variables.userId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not ban this user.')),
  });
}

export function useUnbanUser() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  return useMutation({
    mutationFn: (userId: string) => moderationApi.unban(userId),
    onSuccess: (_data, userId) => {
      showToast('User unbanned.', 'success');
      queryClient.invalidateQueries({ queryKey: ['admin', 'user-detail', userId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not unban this user.')),
  });
}
