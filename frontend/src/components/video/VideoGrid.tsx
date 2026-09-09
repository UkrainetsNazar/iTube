import type { VideoDto, VideoSearchHit } from '@/types';
import { VideoCard } from './VideoCard';
import { VideoGridSkeleton } from '@/components/common/Skeletons';
import { EmptyState } from '@/components/common/EmptyState';

interface VideoGridProps {
  videos: (VideoDto | VideoSearchHit)[] | undefined;
  isLoading: boolean;
  emptyTitle?: string;
  emptyHint?: string;
}

export function VideoGrid({ videos, isLoading, emptyTitle = 'No videos yet', emptyHint }: VideoGridProps) {
  if (isLoading) return <VideoGridSkeleton />;
  if (!videos || videos.length === 0) return <EmptyState title={emptyTitle} hint={emptyHint} />;

  return (
    <div className="grid grid-cols-1 gap-x-4 gap-y-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {videos.map((v, i) => (
        <div
          key={v.id}
          className="animate-fade-in-up"
          // Cap the stagger so a long grid doesn't take forever to finish
          // appearing -- everything past the first ~10 cards animates in
          // together instead of visibly trickling in one by one.
          style={{ animationDelay: `${Math.min(i, 10) * 35}ms` }}
        >
          <VideoCard video={v} authorId={'authorId' in v ? v.authorId : undefined} />
        </div>
      ))}
    </div>
  );
}
