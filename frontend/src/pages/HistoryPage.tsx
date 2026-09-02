import { useState } from 'react';
import { useHistory } from '@/hooks/useVideos';
import { VideoGrid } from '@/components/video/VideoGrid';
import { Pagination } from '@/components/common/Pagination';

const PAGE_SIZE = 24;

export function HistoryPage() {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useHistory(page, PAGE_SIZE);

  return (
    <div>
      <h1 className="mb-5 font-display text-lg font-semibold text-paper">Watch history</h1>
      <VideoGrid
        videos={data?.items}
        isLoading={isLoading}
        emptyTitle="No watch history yet"
        emptyHint="Videos you watch will show up here, most recent first."
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
