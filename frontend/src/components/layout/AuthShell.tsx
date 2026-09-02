import { Link } from 'react-router-dom';
import type { ReactNode } from 'react';

export function AuthShell({ title, subtitle, children }: { title: string; subtitle?: string; children: ReactNode }) {
  return (
    <div className="flex min-h-screen items-center justify-center bg-ink px-4">
      <div className="w-full max-w-sm">
        <Link to="/" className="mb-8 block text-center font-display text-2xl font-semibold text-paper">
          i<span className="text-signal">Tube</span>
        </Link>
        <div className="rounded-card border border-border bg-surface p-6">
          <h1 className="font-display text-lg font-semibold text-paper">{title}</h1>
          {subtitle && <p className="mt-1 text-sm text-paper-dim">{subtitle}</p>}
          <div className="mt-5">{children}</div>
        </div>
      </div>
    </div>
  );
}

export function FormField({
  label,
  ...props
}: React.InputHTMLAttributes<HTMLInputElement> & { label: string }) {
  return (
    <label className="block text-sm">
      <span className="mb-1.5 block text-paper-dim">{label}</span>
      <input
        {...props}
        className="w-full rounded-card border border-border bg-ink px-3 py-2 text-paper placeholder:text-paper-faint focus:border-signal"
      />
    </label>
  );
}

export function SubmitButton({ children, loading }: { children: ReactNode; loading?: boolean }) {
  return (
    <button
      type="submit"
      disabled={loading}
      className="w-full rounded-card bg-signal px-4 py-2.5 text-sm font-medium text-ink hover:bg-signal-hover disabled:opacity-50"
    >
      {loading ? 'Please wait…' : children}
    </button>
  );
}
