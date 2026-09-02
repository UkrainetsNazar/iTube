import { useState, type KeyboardEvent } from 'react';

interface TagInputProps {
  tags: string[];
  onChange: (tags: string[]) => void;
}

export function TagInput({ tags, onChange }: TagInputProps) {
  const [draft, setDraft] = useState('');

  function commitDraft() {
    const value = draft.trim().replace(/,$/, '');
    if (value && !tags.includes(value)) onChange([...tags, value]);
    setDraft('');
  }

  function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
    if (e.key === 'Enter' || e.key === ',') {
      e.preventDefault();
      commitDraft();
    } else if (e.key === 'Backspace' && !draft && tags.length > 0) {
      onChange(tags.slice(0, -1));
    }
  }

  return (
    <div className="flex flex-wrap items-center gap-1.5 rounded-card border border-border bg-ink px-2.5 py-2 focus-within:border-signal">
      {tags.map((tag) => (
        <span key={tag} className="flex items-center gap-1 rounded-full bg-surface-raised px-2.5 py-1 text-xs text-paper">
          {tag}
          <button
            type="button"
            onClick={() => onChange(tags.filter((t) => t !== tag))}
            className="text-paper-faint hover:text-danger"
            aria-label={`Remove ${tag}`}
          >
            ×
          </button>
        </span>
      ))}
      <input
        value={draft}
        onChange={(e) => setDraft(e.target.value)}
        onKeyDown={handleKeyDown}
        onBlur={commitDraft}
        placeholder={tags.length === 0 ? 'Add tags, press Enter' : ''}
        className="min-w-[120px] flex-1 bg-transparent py-0.5 text-sm text-paper placeholder:text-paper-faint outline-none"
      />
    </div>
  );
}
