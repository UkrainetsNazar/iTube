import { useEffect, useState, type FormEvent } from 'react';
import { useSearchParams } from 'react-router-dom';
import { useSearch } from '@/hooks/useSearch';
import { VideoGrid } from '@/components/video/VideoGrid';
import { Pagination } from '@/components/common/Pagination';

const PAGE_SIZE = 24;

export function SearchPage() {
  const [params, setParams] = useSearchParams();
  const q = params.get('q') ?? '';
  const tagsParam = params.get('tags') ?? '';
  const tags = tagsParam ? tagsParam.split(',').map((t) => t.trim()).filter(Boolean) : [];
  const [page, setPage] = useState(1);
  const [tagInput, setTagInput] = useState(tagsParam);
  const hasSearch = q.trim().length > 0 || tags.length > 0;

  useEffect(() => setPage(1), [q, tagsParam]);

  const { data, isLoading } = useSearch(q, tags, page, PAGE_SIZE);

  function handleTagSubmit(e: FormEvent) {
    e.preventDefault();
    const next = new URLSearchParams(params);
    if (tagInput.trim()) next.set('tags', tagInput.trim());
    else next.delete('tags');
    setParams(next);
  }

  return (
    <div>
      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <h1 className="font-display text-lg font-semibold text-paper">
          {q ? (
            <>
              Results for <span className="text-signal">"{q}"</span>
            </>
          ) : tags.length > 0 ? (
            <>
              Results tagged{' '}
              {tags.map((tag, i) => (
                <span key={tag} className="text-signal">
                  #{tag}
                  {i < tags.length - 1 ? ', ' : ''}
                </span>
              ))}
            </>
          ) : (
            'Search'
          )}
        </h1>
      </div>

      {!hasSearch ? (
        <p className="py-24 text-center text-paper-dim">
          Search for something, or filter by tags, to get started.
        </p>
      ) : (
        <>
          <VideoGrid
            videos={data?.hits}
            isLoading={isLoading}
            emptyTitle="No results"
            emptyHint="Try a different search term or a different tag."
          />
          {data && data.hits.length > 0 && (
            <Pagination
              page={page}
              pageSize={PAGE_SIZE}
              itemCount={data.hits.length}
              totalCount={data.totalCount}
              onPageChange={setPage}
            />
          )}
        </>
      )}
    </div>
  );
}