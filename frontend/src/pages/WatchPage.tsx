import { Link, useParams } from 'react-router-dom';
import { useVideo, useRelatedVideos } from '@/hooks/useVideos';
import { VideoPlayer } from '@/components/video/VideoPlayer';
import { ReactionButtons } from '@/components/video/ReactionButtons';
import { SubscribeButton } from '@/components/video/SubscribeButton';
import { CommentsSection } from '@/components/video/CommentsSection';
import { useChannel } from '@/hooks/useChannel';
import { formatCount, formatRelativeDate } from '@/lib/format';
import { ChannelAvatar } from '@/components/common/ChannelAvatar';

export function WatchPage() {
  const { id } = useParams<{ id: string }>();
  const { data: video, isLoading } = useVideo(id);
  const { data: related, isLoading: relatedLoading } = useRelatedVideos(id);
  const { data: channel } = useChannel(video?.authorId);

  if (isLoading) {
    return (
      <div className="mx-auto max-w-6xl animate-pulse">
        <div className="aspect-video w-full rounded-card bg-surface-raised" />
      </div>
    );
  }

  if (!video) {
    return <p className="py-24 text-center text-paper-dim">This video couldn't be found.</p>;
  }

  return (
    <div className="mx-auto grid max-w-6xl grid-cols-1 gap-8 lg:grid-cols-[minmax(0,1fr)_360px]">
      <div className="min-w-0">
        <VideoPlayer videoId={video.id} sources={video.sources} />

        <h1 className="mt-4 font-display text-xl font-semibold text-paper">{video.title}</h1>

        <div className="mt-3 flex flex-wrap items-center justify-between gap-3">
          <div className="flex items-center gap-3">
            <ChannelAvatar channelId={video.authorId} size={44} />
            <div>
              <Link to={`/channel/${video.authorId}`} className="block text-sm font-medium text-paper hover:text-signal">
                {channel?.name ?? 'View channel'}
              </Link>
              {channel && <p className="text-xs text-paper-dim">{formatCount(channel.subscribersCount)} subscribers</p>}
            </div>
            <SubscribeButton channelId={video.authorId} />
          </div>
          <ReactionButtons video={video} />
        </div>

        <div className="mt-4 rounded-card bg-surface p-4">
          <p className="text-xs text-paper-dim">
            {formatCount(video.viewsCount)} views · {formatRelativeDate(video.publishedAt ?? video.createdAt)}
          </p>
          {video.tags.length > 0 && (
            <div className="mt-2 flex flex-wrap gap-1.5">
              {video.tags.map((tag) => (
                <Link
                  key={tag}
                  to={`/search?q=${encodeURIComponent(tag)}&tags=${encodeURIComponent(tag)}`}
                  className="rounded-full bg-surface-raised px-2.5 py-1 text-xs text-paper-dim hover:text-signal"
                >
                  #{tag}
                </Link>
              ))}
            </div>
          )}
          {video.description && (
            <p className="mt-3 whitespace-pre-wrap break-words text-sm text-paper">{video.description}</p>
          )}
        </div>

        <div className="mt-8">
          <CommentsSection videoId={video.id} />
        </div>
      </div>

      <aside className="min-w-0 space-y-4">
        <h2 className="font-display text-sm font-semibold text-paper">Related videos</h2>
        {relatedLoading ? (
          <div className="space-y-3">
            {Array.from({ length: 6 }).map((_, i) => (
              <div key={i} className="h-24 animate-pulse rounded-card bg-surface-raised" />
            ))}
          </div>
        ) : !related || related.length === 0 ? (
          <p className="text-sm text-paper-dim">No related videos right now.</p>
        ) : (
          <div className="space-y-4">
            {related.map((v) => (
              <div key={v.id} className="flex gap-3">
                <Link to={`/watch/${v.id}`} className="aspect-video w-36 shrink-0 overflow-hidden rounded-card bg-surface-raised">
                  {v.thumbnailUrl && <img src={v.thumbnailUrl} alt="" className="h-full w-full object-cover" />}
                </Link>
                <div className="min-w-0">
                  <Link to={`/watch/${v.id}`}>
                    <h3 className="line-clamp-2 text-sm font-medium text-paper">{v.title}</h3>
                  </Link>
                  <p className="mt-1 text-xs text-paper-faint">{formatCount(v.viewsCount)} views</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </aside>
    </div>
  );
}
