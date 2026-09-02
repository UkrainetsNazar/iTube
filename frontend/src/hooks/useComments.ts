import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { commentsApi } from '@/api/comments';
import { useToast } from '@/components/common/ToastProvider';
import { extractApiErrorMessage } from '@/api/client';

export function useComments(videoId: string, page: number, pageSize = 20) {
  return useQuery({
    queryKey: ['comments', videoId, page, pageSize],
    queryFn: () => commentsApi.list(videoId, page, pageSize),
    enabled: Boolean(videoId),
  });
}

export function useAddComment(videoId: string) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: (text: string) => commentsApi.create(videoId, text),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['comments', videoId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not post your comment.')),
  });
}

export function useDeleteComment(videoId: string) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: (commentId: string) => commentsApi.remove(commentId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['comments', videoId] });
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not delete your comment.')),
  });
}
