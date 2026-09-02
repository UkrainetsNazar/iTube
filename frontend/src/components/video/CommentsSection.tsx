import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { useAddComment, useComments, useDeleteComment } from '@/hooks/useComments';
import { useAuthStore } from '@/store/authStore';
import { Pagination } from '@/components/common/Pagination';
import { RowsSkeleton } from '@/components/common/Skeletons';
import { formatRelativeDate } from '@/lib/format';
import { ChannelAvatar } from '@/components/common/ChannelAvatar';

const PAGE_SIZE = 20;

export function CommentsSection({ videoId }: { videoId: string }) {
  const currentUser = useAuthStore((s) => s.currentUser);
  const [page, setPage] = useState(1);
  const [text, setText] = useState('');
  const { data, isLoading } = useComments(videoId, page, PAGE_SIZE);
  const addComment = useAddComment(videoId);
  const deleteComment = useDeleteComment(videoId);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    const trimmed = text.trim();
    if (!trimmed) return;
    addComment.mutate(trimmed, { onSuccess: () => setText('') });
  }

  return (
    <div>
      <h2 className="mb-4 font-display text-base font-semibold text-paper">
        {data ? `${data.totalCount} comment${data.totalCount === 1 ? '' : 's'}` : 'Comments'}
      </h2>

      {currentUser ? (
        <form onSubmit={handleSubmit} className="mb-6 flex gap-3">
          {currentUser && <ChannelAvatar channelId={currentUser.id} size={36} linkToChannel={false} />}
          <div className="flex-1">
            <input
              value={text}
              onChange={(e) => setText(e.target.value)}
              placeholder="Add a comment…"
              className="w-full border-b border-border bg-transparent pb-1.5 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
            />
            {text.trim() && (
              <div className="mt-2 flex justify-end gap-2">
                <button
                  type="button"
                  onClick={() => setText('')}
                  className="rounded-card px-3 py-1.5 text-xs text-paper-dim hover:bg-surface"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={addComment.isPending}
                  className="rounded-card bg-signal px-3 py-1.5 text-xs font-medium text-ink hover:bg-signal-hover disabled:opacity-50"
                >
                  Comment
                </button>
              </div>
            )}
          </div>
        </form>
      ) : (
        <p className="mb-6 text-sm text-paper-dim">
          <Link to="/login" className="text-signal hover:text-signal-hover">
            Sign in
          </Link>{' '}
          to leave a comment.
        </p>
      )}

      {isLoading ? (
        <RowsSkeleton count={4} />
      ) : !data || data.items.length === 0 ? (
        <p className="text-sm text-paper-dim">No comments yet. Be the first to say something.</p>
      ) : (
        <ul className="space-y-5">
          {data.items.map((c) => (
            <li key={c.id} className="flex gap-3">
              <ChannelAvatar channelId={c.authorId} size={36} />
              <div className="min-w-0 flex-1">
                <div className="flex items-center gap-2 text-xs text-paper-dim">
                  <Link to={`/channel/${c.authorId}`} className="font-medium text-paper hover:text-signal">
                    View profile
                  </Link>
                  <span>{formatRelativeDate(c.createdAt)}</span>
                </div>
                <p className="mt-1 whitespace-pre-wrap break-words text-sm text-paper">{c.text}</p>
                {currentUser?.id === c.authorId && (
                  <button
                    onClick={() => deleteComment.mutate(c.id)}
                    disabled={deleteComment.isPending}
                    className="mt-1 text-xs text-paper-faint hover:text-danger"
                  >
                    Delete
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>
      )}

      {data && (
        <Pagination
          page={page}
          pageSize={PAGE_SIZE}
          itemCount={data.items.length}
          totalCount={data.totalCount}
          onPageChange={setPage}
        />
      )}
    </div>
  );
}
