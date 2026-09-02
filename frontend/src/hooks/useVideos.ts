import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { videosApi, type VideoListResult } from '@/api/videos';
import { recommendationsApi } from '@/api/recommendations';
import { useAuthStore } from '@/store/authStore';
import { useToast } from '@/components/common/ToastProvider';
import { extractApiErrorMessage } from '@/api/client';
import type { ReactionType, Visibility } from '@/types';

const DEFAULT_PAGE_SIZE = 24;

export function useHomeFeed(page: number, pageSize = DEFAULT_PAGE_SIZE) {
  const currentUser = useAuthStore((s) => s.currentUser);

  return useQuery({
    queryKey: ['home-feed', currentUser?.id ?? 'anon', currentUser ? 1 : page, pageSize],
    queryFn: (): Promise<VideoListResult> =>
      currentUser
        ? recommendationsApi.get({ limit: pageSize }).then((items) => ({ items, page: 1, pageSize }))
        : videosApi.feed(page, pageSize),
  });
}

export function useVideo(id: string | undefined) {
  return useQuery({
    queryKey: ['video', id],
    queryFn: () => videosApi.getById(id as string),
    enabled: Boolean(id),
  });
}

export function useRelatedVideos(videoId: string | undefined, limit = 12) {
  return useQuery({
    queryKey: ['recommendations', 'related', videoId, limit],
    queryFn: () => recommendationsApi.get({ videoId, limit }),
    enabled: Boolean(videoId),
  });
}

export function useReactToVideo(videoId: string) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: (type: ReactionType) => videosApi.react(videoId, type),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['video', videoId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not register your reaction.')),
  });
}

export function useRecordView(videoId: string) {
  return useMutation({
    mutationFn: (watchedSeconds: number) => videosApi.recordView(videoId, watchedSeconds),
  });
}

export function useLikedVideos(page: number, pageSize = DEFAULT_PAGE_SIZE) {
  return useQuery({
    queryKey: ['videos', 'liked', page, pageSize],
    queryFn: () => videosApi.liked(page, pageSize),
  });
}

export function useHistory(page: number, pageSize = DEFAULT_PAGE_SIZE) {
  return useQuery({
    queryKey: ['videos', 'history', page, pageSize],
    queryFn: () => videosApi.history(page, pageSize),
  });
}

export function useSubscriptionsFeed(page: number, pageSize = DEFAULT_PAGE_SIZE) {
  return useQuery({
    queryKey: ['videos', 'subscriptions-feed', page, pageSize],
    queryFn: () => videosApi.subscriptionsFeed(page, pageSize),
  });
}

export function useChannelVideos(channelId: string | undefined, page: number, pageSize = DEFAULT_PAGE_SIZE) {
  return useQuery({
    queryKey: ['videos', 'channel', channelId, page, pageSize],
    queryFn: () => videosApi.channelVideos(channelId as string, page, pageSize),
    enabled: Boolean(channelId),
  });
}

export function useMyVideos(page: number, pageSize = DEFAULT_PAGE_SIZE) {
  return useQuery({
    queryKey: ['videos', 'mine', page, pageSize],
    queryFn: () => videosApi.myVideos(page, pageSize),
  });
}

export function usePublishVideo() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: ({ id, visibility }: { id: string; visibility: Visibility }) => videosApi.publish(id, visibility),
    onSuccess: () => {
      showToast('Video published.', 'success');
      queryClient.invalidateQueries({ queryKey: ['videos', 'mine'] });
    },
    onError: (err) =>
      showToast(
        extractApiErrorMessage(
          err,
          'Could not publish. Media may still be processing -- sources are empty until that finishes.'
        )
      ),
  });
}

export function useSetVisibility() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: ({ id, visibility }: { id: string; visibility: Visibility }) =>
      videosApi.setVisibility(id, visibility),
    onSuccess: () => {
      showToast('Visibility updated.', 'success');
      queryClient.invalidateQueries({ queryKey: ['videos', 'mine'] });
    },
    onError: (err) =>
      showToast(extractApiErrorMessage(err, 'Could not change visibility. The video may not be published yet.')),
  });
}

export function useDeleteVideo() {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: (id: string) => videosApi.remove(id),
    onSuccess: () => {
      showToast('Video deleted.', 'success');
      queryClient.invalidateQueries({ queryKey: ['videos', 'mine'] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not delete this video.')),
  });
}
