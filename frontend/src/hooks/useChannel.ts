import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { channelsApi, type UpdateChannelPayload } from '@/api/channels';
import { subscriptionsApi } from '@/api/subscriptions';
import { useAuthStore } from '@/store/authStore';
import { useToast } from '@/components/common/ToastProvider';
import { extractApiErrorMessage } from '@/api/client';

export function useChannel(channelId: string | undefined) {
  return useQuery({
    queryKey: ['channel', channelId],
    queryFn: () => channelsApi.getById(channelId as string),
    enabled: Boolean(channelId),
  });
}

export function useUpdateChannel(channelId: string) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: (payload: UpdateChannelPayload) => channelsApi.update(channelId, payload),
    onSuccess: () => {
      showToast('Channel updated.', 'success');
      queryClient.invalidateQueries({ queryKey: ['channel', channelId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not update your channel.')),
  });
}

/** Subscribe-state check + toggle for a channel. No-ops (disabled) when logged out. */
export function useSubscription(channelId: string | undefined) {
  const currentUser = useAuthStore((s) => s.currentUser);
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const enabled = Boolean(channelId && currentUser);

  const checkQuery = useQuery({
    queryKey: ['subscription-check', channelId],
    queryFn: () => subscriptionsApi.check(channelId as string),
    enabled,
  });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: ['subscription-check', channelId] });
    queryClient.invalidateQueries({ queryKey: ['channel', channelId] });
  };

  const subscribe = useMutation({
    mutationFn: () => subscriptionsApi.subscribe(channelId as string),
    onSuccess: invalidate,
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not subscribe.')),
  });

  const unsubscribe = useMutation({
    mutationFn: () => subscriptionsApi.unsubscribe(channelId as string),
    onSuccess: invalidate,
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not unsubscribe.')),
  });

  return {
    isSubscribed: checkQuery.data ?? false,
    isLoading: checkQuery.isLoading,
    enabled,
    toggle: () => (checkQuery.data ? unsubscribe.mutate() : subscribe.mutate()),
    isToggling: subscribe.isPending || unsubscribe.isPending,
  };
}

export function useMySubscriptions() {
  const currentUser = useAuthStore((s) => s.currentUser);
  return useQuery({
    queryKey: ['subscriptions-me'],
    queryFn: () => subscriptionsApi.mine(),
    enabled: Boolean(currentUser),
  });
}
