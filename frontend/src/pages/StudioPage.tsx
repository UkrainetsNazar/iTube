import { useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useDeleteVideo, useMyVideos, usePublishVideo, useSetVisibility } from '@/hooks/useVideos';
import { UploadFlow } from '@/components/upload/UploadFlow';
import { ConfirmDialog } from '@/components/common/ConfirmDialog';
import { Pagination } from '@/components/common/Pagination';
import { RowsSkeleton } from '@/components/common/Skeletons';
import { EmptyState } from '@/components/common/EmptyState';
import { formatCount, formatRelativeDate } from '@/lib/format';
import type { VideoDto, Visibility } from '@/types';

const PAGE_SIZE = 20;

const statusStyles: Record<VideoDto['status'], string> = {
  Draft: 'bg-surface-raised text-paper-dim',
  Published: 'bg-moss/15 text-moss',
  Deleted: 'bg-danger/15 text-danger',
};

const visibilityStyles: Record<Visibility, string> = {
  Public: 'border-moss/40 text-moss',
  Private: 'border-border text-paper-dim',
};

export function StudioPage() {
  const [params, setParams] = useSearchParams();
  const [page, setPage] = useState(1);
  const { data, isLoading } = useMyVideos(page, PAGE_SIZE);
  const setVisibility = useSetVisibility();
  const publish = usePublishVideo();
  const deleteVideo = useDeleteVideo();
  const [draftVisibility, setDraftVisibility] = useState<Record<string, Visibility>>({});
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);

  const showUpload = params.get('upload') === '1';

  function closeUpload() {
    const next = new URLSearchParams(params);
    next.delete('upload');
    setParams(next, { replace: true });
  }

  function handleConfirmDelete() {
    if (!confirmDeleteId) return;
    deleteVideo.mutate(confirmDeleteId, {
      onSettled: () => setConfirmDeleteId(null),
    });
  }

  return (
    <div>
      <div className="mb-5 flex items-center justify-between">
        <h1 className="font-display text-lg font-semibold text-paper">Your videos</h1>
        <button
          onClick={() => setParams({ upload: '1' })}
          className="rounded-card bg-signal px-4 py-2 text-sm font-medium text-ink hover:bg-signal-hover"
        >
          Upload video
        </button>
      </div>

      {isLoading ? (
        <RowsSkeleton />
      ) : !data || data.items.length === 0 ? (
        <EmptyState title="No videos yet" hint="Upload your first video to see it here." />
      ) : (
        <div className="overflow-x-auto rounded-card border border-border">
          <table className="w-full min-w-[720px] text-left text-sm">
            <thead className="border-b border-border text-xs uppercase tracking-wide text-paper-faint">
              <tr>
                <th className="px-4 py-3 font-medium">Video</th>
                <th className="px-4 py-3 font-medium">Status</th>
                <th className="px-4 py-3 font-medium">Visibility</th>
                <th className="px-4 py-3 font-medium">Views</th>
                <th className="px-4 py-3 font-medium">Uploaded</th>
                <th className="px-4 py-3 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((video) => (
                <tr key={video.id} className="border-b border-border last:border-0">
                  <td className="max-w-xs px-4 py-3">
                    <Link to={`/watch/${video.id}`} className="line-clamp-2 font-medium text-paper hover:text-signal">
                      {video.title}
                    </Link>
                  </td>
                  <td className="px-4 py-3">
                    <span className={`rounded-full px-2.5 py-1 text-xs ${statusStyles[video.status]}`}>{video.status}</span>
                  </td>
                  <td className="px-4 py-3">
                    {video.status === 'Published' ? (
                      <select
                        value={video.visibility}
                        onChange={(e) =>
                          setVisibility.mutate({ id: video.id, visibility: e.target.value as Visibility })
                        }
                        className={`rounded-card border bg-transparent px-2 py-1 text-xs ${visibilityStyles[video.visibility]}`}
                      >
                        <option value="Public">Public</option>
                        <option value="Private">Private</option>
                      </select>
                    ) : video.status === 'Draft' ? (
                      <select
                        value={draftVisibility[video.id] ?? 'Public'}
                        onChange={(e) =>
                          setDraftVisibility((prev) => ({ ...prev, [video.id]: e.target.value as Visibility }))
                        }
                        title="Visibility to publish with"
                        className="rounded-card border border-border bg-transparent px-2 py-1 text-xs text-paper-dim"
                      >
                        <option value="Public">Public</option>
                        <option value="Private">Private</option>
                      </select>
                    ) : (
                      <span className="text-xs text-paper-faint">—</span>
                    )}
                  </td>
                  <td className="px-4 py-3 text-paper-dim">{formatCount(video.viewsCount)}</td>
                  <td className="px-4 py-3 text-paper-dim">{formatRelativeDate(video.createdAt)}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-3">
                      {video.status === 'Draft' && (
                        <button
                          onClick={() =>
                            publish.mutate({ id: video.id, visibility: draftVisibility[video.id] ?? 'Public' })
                          }
                          disabled={video.sources.length === 0 || publish.isPending}
                          title={video.sources.length === 0 ? 'Still processing -- no sources yet' : undefined}
                          className="text-xs text-signal hover:text-signal-hover disabled:cursor-not-allowed disabled:text-paper-faint"
                        >
                          Publish
                        </button>
                      )}
                      <button
                        onClick={() => setConfirmDeleteId(video.id)}
                        className="text-xs text-paper-faint hover:text-danger"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {data && data.items.length > 0 && (
        <Pagination
          page={page}
          pageSize={PAGE_SIZE}
          itemCount={data.items.length}
          totalCount={data.totalCount}
          onPageChange={setPage}
        />
      )}

      {showUpload && <UploadFlow onClose={closeUpload} />}

      {confirmDeleteId && (
        <ConfirmDialog
          title="Delete this video?"
          message="This can't be undone. The video, its comments, and its stats will be permanently removed."
          confirmLabel="Delete video"
          danger
          isLoading={deleteVideo.isPending}
          onConfirm={handleConfirmDelete}
          onCancel={() => setConfirmDeleteId(null)}
        />
      )}
    </div>
  );
}