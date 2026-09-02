interface EmptyStateProps {
  title: string;
  hint?: string;
  action?: React.ReactNode;
}

export function EmptyState({ title, hint, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-2 py-24 text-center">
      <p className="font-display text-lg text-paper">{title}</p>
      {hint && <p className="max-w-sm text-sm text-paper-dim">{hint}</p>}
      {action && <div className="mt-3">{action}</div>}
    </div>
  );
}
