import { Link } from 'react-router-dom';

export function NotFoundPage() {
  return (
    <div className="flex min-h-[60vh] flex-col items-center justify-center gap-3 text-center">
      <p className="font-display text-4xl font-semibold text-paper">404</p>
      <p className="text-paper-dim">This page doesn't exist.</p>
      <Link to="/" className="mt-2 text-sm text-signal hover:text-signal-hover">
        Back to home
      </Link>
    </div>
  );
}
