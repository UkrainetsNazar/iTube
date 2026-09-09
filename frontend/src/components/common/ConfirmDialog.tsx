import { createPortal } from 'react-dom';

interface ConfirmDialogProps {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  danger?: boolean;
  isLoading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmDialog({
  title,
  message,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  danger = false,
  isLoading = false,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  return createPortal(
    <div
      className="fixed inset-0 z-50 flex animate-fade-in items-center justify-center bg-black/85 p-4 backdrop-blur-sm"
      onClick={() => !isLoading && onCancel()}
      role="alertdialog"
      aria-modal="true"
      aria-labelledby="confirm-dialog-title"
    >
      <div
        className="w-full max-w-sm animate-scale-in rounded-card border border-border bg-surface p-6 shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-start gap-3">
          {danger && (
            <div className="mt-0.5 flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-danger/15 text-danger">
              <svg width="18" height="18" viewBox="0 0 20 20" fill="none">
                <path
                  d="M10 7v4M10 14h.01M8.7 3.3l-6 10.5A1.5 1.5 0 004 16h12a1.5 1.5 0 001.3-2.2l-6-10.5a1.5 1.5 0 00-2.6 0z"
                  stroke="currentColor"
                  strokeWidth="1.5"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
            </div>
          )}
          <div className="min-w-0">
            <h2 id="confirm-dialog-title" className="font-display text-base font-semibold text-paper">
              {title}
            </h2>
            <p className="mt-1.5 text-sm text-paper-dim">{message}</p>
          </div>
        </div>

        <div className="mt-6 flex justify-end gap-2">
          <button
            onClick={onCancel}
            disabled={isLoading}
            className="rounded-card border border-border px-4 py-2 text-sm font-medium text-paper transition-all duration-150 hover:border-signal active:scale-[0.97] disabled:opacity-50"
          >
            {cancelLabel}
          </button>
          <button
            onClick={onConfirm}
            disabled={isLoading}
            className={`rounded-card px-4 py-2 text-sm font-medium transition-all duration-150 active:scale-[0.97] disabled:opacity-50 ${
              danger ? 'bg-danger text-ink hover:bg-danger-hover' : 'bg-signal text-ink hover:bg-signal-hover'
            }`}
          >
            {isLoading ? 'Please wait…' : confirmLabel}
          </button>
        </div>
      </div>
    </div>,
    document.body
  );
}