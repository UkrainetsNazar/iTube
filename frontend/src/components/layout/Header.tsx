import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { SearchBar } from './SearchBar';
import { useAuthStore } from '@/store/authStore';
import { useLogout } from '@/hooks/useAuth';

interface HeaderProps {
  onMenuClick: () => void;
}

export function Header({ onMenuClick }: HeaderProps) {
  const currentUser = useAuthStore((s) => s.currentUser);
  const logout = useLogout();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <header className="sticky top-0 z-40 flex h-16 items-center gap-4 border-b border-border bg-ink/95 px-4 backdrop-blur">
      <button
        onClick={onMenuClick}
        className="rounded-card p-2 text-paper hover:bg-surface lg:hidden"
        aria-label="Toggle navigation"
      >
        <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
          <path d="M2 5h16M2 10h16M2 15h16" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" />
        </svg>
      </button>

      <Link to="/" className="shrink-0 font-display text-xl font-semibold tracking-tight text-paper">
        i<span className="text-signal">Tube</span>
      </Link>

      <div className="hidden flex-1 justify-center sm:flex">
        <SearchBar />
      </div>

      <div className="ml-auto flex items-center gap-3">
        {currentUser ? (
          <>
            <button
              onClick={() => navigate('/studio?upload=1')}
              className="hidden rounded-card bg-signal px-4 py-2 text-sm font-medium text-ink hover:bg-signal-hover sm:block"
            >
              Upload
            </button>
            <div className="relative">
              <button
                onClick={() => setMenuOpen((v) => !v)}
                className="flex h-9 w-9 items-center justify-center rounded-full bg-surface-raised text-sm font-medium text-paper hover:bg-surface-hover"
                aria-label="Account menu"
              >
                {currentUser.email.charAt(0).toUpperCase()}
              </button>
              {menuOpen && (
                <div
                  className="absolute right-0 mt-2 w-48 overflow-hidden rounded-card border border-border bg-surface-raised shadow-xl"
                  onMouseLeave={() => setMenuOpen(false)}
                >
                                    <div className="truncate border-b border-border px-4 py-3 text-xs text-paper-dim">
                    {currentUser.email}
                  </div>
                  <Link
                    to={`/channel/${currentUser.id}`}
                    onClick={() => setMenuOpen(false)}
                    className="block px-4 py-2.5 text-sm text-paper hover:bg-surface-hover"
                  >
                    Your channel
                  </Link>
                  <Link
                    to="/studio"
                    onClick={() => setMenuOpen(false)}
                    className="block px-4 py-2.5 text-sm text-paper hover:bg-surface-hover"
                  >
                    Studio
                  </Link>
                  <Link
                    to="/settings"
                    onClick={() => setMenuOpen(false)}
                    className="block px-4 py-2.5 text-sm text-paper hover:bg-surface-hover"
                  >
                    Settings
                  </Link>
                  {(currentUser.role === 'Admin' || currentUser.role === 'Moderator') && (
                    <Link
                      to="/admin"
                      onClick={() => setMenuOpen(false)}
                      className="block px-4 py-2.5 text-sm text-paper hover:bg-surface-hover"
                    >
                      Admin panel
                    </Link>
                  )}
                  <button
                    onClick={() => {
                      setMenuOpen(false);
                      logout.mutate();
                    }}
                    className="block w-full px-4 py-2.5 text-left text-sm text-danger hover:bg-surface-hover"
                  >
                    Log out
                  </button>
                </div>
              )}
            </div>
          </>
        ) : (
          <Link
            to="/login"
            className="rounded-card border border-border px-4 py-2 text-sm font-medium text-paper hover:border-signal"
          >
            Sign in
          </Link>
        )}
      </div>
    </header>
  );
}
