import { createContext, useCallback, useContext, useRef, useState, type ReactNode } from 'react';

interface Toast {
  id: number;
  message: string;
  tone: 'error' | 'success' | 'info';
  leaving: boolean;
}

interface ToastContextValue {
  showToast: (message: string, tone?: Toast['tone']) => void;
}

const ToastContext = createContext<ToastContextValue | null>(null);

const DISPLAY_MS = 5000;
const EXIT_ANIMATION_MS = 200;

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([]);
  const idRef = useRef(0);

  const showToast = useCallback((message: string, tone: Toast['tone'] = 'error') => {
    const id = ++idRef.current;
    setToasts((prev) => [...prev, { id, message, tone, leaving: false }]);

    // Two-phase removal: flip `leaving` first so the exit transition can
    // play, then actually drop it from state once that transition has had
    // time to finish. Removing immediately would just pop it out of
    // existence with no exit animation.
    setTimeout(() => {
      setToasts((prev) => prev.map((t) => (t.id === id ? { ...t, leaving: true } : t)));
      setTimeout(() => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
      }, EXIT_ANIMATION_MS);
    }, DISPLAY_MS);
  }, []);

  return (
    <ToastContext.Provider value={{ showToast }}>
      {children}
      <div className="fixed bottom-4 right-4 z-50 flex w-80 max-w-[90vw] flex-col gap-2">
        {toasts.map((t) => (
          <div
            key={t.id}
            role="status"
            className={`rounded-card border px-4 py-3 text-sm shadow-lg backdrop-blur transition-all duration-200 ease-smooth ${
              t.leaving ? 'translate-x-2 opacity-0' : 'animate-slide-in-right opacity-100'
            } ${
              t.tone === 'error'
                ? 'bg-danger/10 border-danger/40 text-paper'
                : t.tone === 'success'
                  ? 'bg-moss/10 border-moss/40 text-paper'
                  : 'bg-surface-raised border-border text-paper'
            }`}
          >
            {t.message}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast(): ToastContextValue {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error('useToast must be used within a ToastProvider');
  return ctx;
}
