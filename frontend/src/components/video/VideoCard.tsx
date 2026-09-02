import { Link } from 'react-router-dom';
import type { VideoDto, VideoSearchHit } from '@/types';
import { formatCount, formatRelativeDate } from '@/lib/format';
import { useChannel } from '@/hooks/useChannel';
import { ChannelAvatar } from '@/components/common/ChannelAvatar';

const PLACEHOLDER_THUMB =
  'data:image/svg+xml;utf8,' +
  encodeURIComponent(
    `<svg xmlns="http://www.w3.org/2000/svg" width="320" height="180" viewBox="0 0 320 180"><rect width="320" height="180" fill="#23262E"/><path d="M132 65l60 25-60 25V65z" fill="#4A4E58"/></svg>`
  );

interface VideoCardProps {
  video: VideoDto | VideoSearchHit;
  authorId?: string;
}

export function VideoCard({ video, authorId }: VideoCardProps) {
  const { data: channel } = useChannel(authorId);

  return (
    <div className="group">
      <Link to={`/watch/${video.id}`} className="block">
        <div className="aspect-video w-full overflow-hidden rounded-card bg-surface-raised">
          <img
            src={video.thumbnailUrl ?? PLACEHOLDER_THUMB}
            alt=""
            className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-[1.02]"
            loading="lazy"
          />
        </div>
      </Link>
      <div className="mt-3 flex gap-3">
        {authorId ? (
          <ChannelAvatar channelId={authorId} size={36} />
        ) : (
          <div className="h-9 w-9 shrink-0 rounded-full bg-surface-raised" aria-hidden />
        )}
        <div className="min-w-0">
          <Link to={`/watch/${video.id}`}>
            <h3 className="line-clamp-2 text-sm font-medium text-paper">{video.title}</h3>
          </Link>
          {authorId && (
            <Link to={`/channel/${authorId}`} className="text-xs text-paper-dim hover:text-paper">
              {channel?.name ?? '\u00A0'}
            </Link>
          )}
          <p className="text-xs text-paper-faint">
            {formatCount(video.viewsCount)} views · {formatRelativeDate(video.createdAt)}
          </p>
        </div>
      </div>
    </div>
  );
}