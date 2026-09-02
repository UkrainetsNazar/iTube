import { useState } from 'react';
import { useLikedVideos } from '@/hooks/useVideos';
import { VideoGrid } from '@/components/video/VideoGrid';
import { Pagination } from '@/components/common/Pagination';

const PAGE_SIZE = 24;

export function LikedPage() {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useLikedVideos(page, PAGE_SIZE);

  return (
    <div>
      <h1 className="mb-5 font-display text-lg font-semibold text-paper">Liked videos</h1>
      <VideoGrid
        videos={data?.items}
        isLoading={isLoading}
        emptyTitle="No liked videos yet"
        emptyHint="Videos you like will show up here."
      />
      {data && data.items.length > 0 && (
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
