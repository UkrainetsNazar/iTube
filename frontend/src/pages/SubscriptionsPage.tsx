import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useSubscriptionsFeed } from '@/hooks/useVideos';
import { useMySubscriptions } from '@/hooks/useChannel';
import { VideoGrid } from '@/components/video/VideoGrid';
import { Pagination } from '@/components/common/Pagination';

const PAGE_SIZE = 24;

export function SubscriptionsPage() {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useSubscriptionsFeed(page, PAGE_SIZE);
  const { data: channels } = useMySubscriptions();

  return (
    <div className="grid grid-cols-1 gap-8 lg:grid-cols-[minmax(0,1fr)_240px]">
      <div className="min-w-0">
        <h1 className="mb-5 font-display text-lg font-semibold text-paper">Subscriptions</h1>
        <VideoGrid
          videos={data?.items}
          isLoading={isLoading}
          emptyTitle="No new videos"
          emptyHint="New uploads from channels you follow will show up here."
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
      <aside>
        <h2 className="mb-3 font-display text-sm font-semibold text-paper">Channels you follow</h2>
        {!channels || channels.length === 0 ? (
          <p className="text-sm text-paper-dim">You haven't subscribed to any channels yet.</p>
        ) : (
          <ul className="space-y-2">
            {channels.map((c) => (
              <li key={c.channelId}>
                <Link
                  to={`/channel/${c.channelId}`}
                  className="flex items-center gap-2.5 rounded-card px-2 py-1.5 text-sm text-paper hover:bg-surface"
                >
                  <span className="h-7 w-7 shrink-0 rounded-full bg-surface-raised" />
                  View channel
                </Link>
              </li>
            ))}
          </ul>
        )}
      </aside>
    </div>
  );
}
