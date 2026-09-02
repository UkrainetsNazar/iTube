import { useState } from 'react';
import { VideoGrid } from '@/components/video/VideoGrid';
import { Pagination } from '@/components/common/Pagination';
import { useHomeFeed } from '@/hooks/useVideos';
import { useAuthStore } from '@/store/authStore';

const PAGE_SIZE = 24;

export function HomePage() {
  const [page, setPage] = useState(1);
  const currentUser = useAuthStore((s) => s.currentUser);
  const { data, isLoading } = useHomeFeed(page, PAGE_SIZE);

  return (
    <div>
      <h1 className="mb-5 font-display text-lg font-semibold text-paper">
        {currentUser ? 'Recommended for you' : 'Popular now'}
      </h1>
      <VideoGrid
        videos={data?.items}
        isLoading={isLoading}
        emptyTitle="Nothing to show yet"
        emptyHint="Videos will show up here once they're published."
      />
      {/* GET /recommendations has no `page` param, so the personalized feed can't be paged -- see useHomeFeed. */}
      {!currentUser && data && data.items.length > 0 && (
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
