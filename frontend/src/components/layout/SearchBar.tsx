import { useState, type FormEvent } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

export function SearchBar() {
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const [value, setValue] = useState(params.get('q') ?? '');

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    const trimmed = value.trim();
    if (!trimmed) return;
    navigate(`/search?q=${encodeURIComponent(trimmed)}`);
  }

  return (
    <form onSubmit={handleSubmit} className="flex w-full max-w-xl">
      <input
        value={value}
        onChange={(e) => setValue(e.target.value)}
        type="search"
        placeholder="Search videos"
        aria-label="Search videos"
        className="w-full rounded-l-card border border-border bg-surface px-4 py-2 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
      />
      <button
        type="submit"
        className="rounded-r-card border border-l-0 border-border bg-surface-raised px-4 text-sm text-paper hover:bg-surface-hover"
      >
        Search
      </button>
    </form>
  );
}
