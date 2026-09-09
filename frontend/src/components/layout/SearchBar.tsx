import { useEffect, useRef, useState, type FormEvent } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

function TagIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 20 20" fill="none">
      <path
        d="M9.5 3H4a1 1 0 00-1 1v5.5a1 1 0 00.3.7l7 7a1 1 0 001.4 0l5.5-5.5a1 1 0 000-1.4l-7-7A1 1 0 009.5 3z"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinejoin="round"
      />
      <circle cx="7" cy="7" r="1" fill="currentColor" />
    </svg>
  );
}

export function SearchBar() {
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const [value, setValue] = useState(params.get('q') ?? '');
  const [tagInput, setTagInput] = useState(params.get('tags') ?? '');
  const [tagPanelOpen, setTagPanelOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setValue(params.get('q') ?? '');
    setTagInput(params.get('tags') ?? '');
  }, [params]);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setTagPanelOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  function runSearch() {
    const q = value.trim();
    const tags = tagInput.trim();
    if (!q && !tags) return;
    const next = new URLSearchParams();
    if (q) next.set('q', q);
    if (tags) next.set('tags', tags);
    navigate(`/search?${next.toString()}`);
    setTagPanelOpen(false);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    runSearch();
  }

  const hasTags = tagInput.trim().length > 0;

  return (
    <div ref={containerRef} className="relative w-full max-w-xl">
      <form onSubmit={handleSubmit} className="flex w-full">
        <input
          value={value}
          onChange={(e) => setValue(e.target.value)}
          type="search"
          placeholder="Search videos"
          aria-label="Search videos"
          className="w-full rounded-l-card border border-border bg-surface px-4 py-2 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
        />
        <button
          type="button"
          onClick={() => setTagPanelOpen((v) => !v)}
          title="Filter by tags"
          aria-label="Filter by tags"
          aria-expanded={tagPanelOpen}
          className={`flex items-center gap-1 border-y border-border px-3 text-sm transition-colors ${
            hasTags ? 'bg-signal/15 text-signal' : 'bg-surface text-paper-dim hover:text-paper'
          }`}
        >
          <TagIcon />
          {hasTags && <span className="h-1.5 w-1.5 rounded-full bg-signal" aria-hidden />}
        </button>
        <button
          type="submit"
          className="rounded-r-card border border-l-0 border-border bg-surface-raised px-4 text-sm text-paper hover:bg-surface-hover"
        >
          Search
        </button>
      </form>

      {tagPanelOpen && (
        <div className="absolute left-0 right-0 top-full z-20 mt-2 animate-fade-in-up rounded-card border border-border bg-surface-raised p-3 shadow-xl">
          <label className="mb-1.5 block text-xs text-paper-dim">Filter by tags (comma-separated)</label>
          <div className="flex gap-2">
            <input
              value={tagInput}
              onChange={(e) => setTagInput(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), runSearch())}
              placeholder="e.g. cooking, italian"
              autoFocus
              className="w-full rounded-card border border-border bg-ink px-3 py-1.5 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
            />
            <button
              type="button"
              onClick={runSearch}
              className="shrink-0 rounded-card bg-signal px-3 py-1.5 text-sm font-medium text-ink hover:bg-signal-hover"
            >
              Apply
            </button>
          </div>
          <p className="mt-2 text-xs text-paper-faint">
            Combine with a search term above, or leave it blank and search by tags alone.
          </p>
        </div>
      )}
    </div>
  );
}