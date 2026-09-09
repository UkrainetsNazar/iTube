interface PaginationProps {
  page: number;
  pageSize: number;
  /** Number of items returned in the current page's response. */
  itemCount: number;
  /** Present when the endpoint returns `{ items, totalCount, ... }`; undefined for bare-array endpoints. */
  totalCount?: number;
  onPageChange: (page: number) => void;
}

/**
 * Works with both response shapes the backend uses:
 *  - `{ items, totalCount, page, pageSize }` -> exact page count, "Page X of Y".
 *  - bare array -> no totalCount, so "Next" is enabled only when the current
 *    page came back full (a short page implies it's the last one).
 */
export function Pagination({ page, pageSize, itemCount, totalCount, onPageChange }: PaginationProps) {
  const hasKnownTotal = totalCount !== undefined;
  const totalPages = hasKnownTotal ? Math.max(1, Math.ceil(totalCount / pageSize)) : undefined;
  const canGoNext = hasKnownTotal ? page < (totalPages as number) : itemCount === pageSize;
  const canGoPrev = page > 1;

  if (hasKnownTotal && totalCount === 0) return null;

  return (
    <div className="flex items-center justify-center gap-4 py-8 text-sm">
      <button
        onClick={() => onPageChange(page - 1)}
        disabled={!canGoPrev}
        className="rounded-card border border-border px-4 py-2 text-paper transition-all duration-150 hover:border-signal active:scale-[0.96] disabled:cursor-not-allowed disabled:opacity-30 disabled:active:scale-100"
      >
        Previous
      </button>
      <span className="text-paper-dim">{hasKnownTotal ? `Page ${page} of ${totalPages}` : `Page ${page}`}</span>
      <button
        onClick={() => onPageChange(page + 1)}
        disabled={!canGoNext}
        className="rounded-card border border-border px-4 py-2 text-paper transition-all duration-150 hover:border-signal active:scale-[0.96] disabled:cursor-not-allowed disabled:opacity-30 disabled:active:scale-100"
      >
        Next
      </button>
    </div>
  );
}
