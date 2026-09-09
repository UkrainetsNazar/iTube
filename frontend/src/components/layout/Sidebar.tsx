import { NavLink } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';

const iconClass = 'h-5 w-5 shrink-0';

function HomeIcon() {
  return (
    <svg className={iconClass} viewBox="0 0 20 20" fill="none">
      <path d="M3 9.5L10 3l7 6.5M5 8v8h10V8" stroke="currentColor" strokeWidth="1.5" strokeLinejoin="round" />
    </svg>
  );
}
function BellIcon() {
  return (
    <svg className={iconClass} viewBox="0 0 20 20" fill="none">
      <path
        d="M5 8a5 5 0 0110 0c0 3 1 4 1 4H4s1-1 1-4zM8 15a2 2 0 004 0"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinejoin="round"
      />
    </svg>
  );
}
function ThumbIcon() {
  return (
    <svg className={iconClass} viewBox="0 0 20 20" fill="none">
      <path
        d="M7 9v8H4V9h3zm0 0l3-6 1 1-1 4h5a1.5 1.5 0 011.4 2l-1.6 5a1.5 1.5 0 01-1.4 1H7"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinejoin="round"
      />
    </svg>
  );
}
function HistoryIcon() {
  return (
    <svg className={iconClass} viewBox="0 0 20 20" fill="none">
      <path
        d="M10 5v5l3 2M17 10a7 7 0 11-2.3-5.2"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}
function StudioIcon() {
  return (
    <svg className={iconClass} viewBox="0 0 20 20" fill="none">
      <rect x="3" y="4" width="14" height="10" rx="1.5" stroke="currentColor" strokeWidth="1.5" />
      <path d="M7 17h6" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
    </svg>
  );
}
function ShieldIcon() {
  return (
    <svg className={iconClass} viewBox="0 0 20 20" fill="none">
      <path d="M10 3l6 2v5c0 4-2.5 6.5-6 7-3.5-.5-6-3-6-7V5l6-2z" stroke="currentColor" strokeWidth="1.5" strokeLinejoin="round" />
    </svg>
  );
}

interface NavItem {
  to: string;
  label: string;
  icon: () => JSX.Element;
  requiresAuth?: boolean;
  requiresAdmin?: boolean;
}

const navItems: NavItem[] = [
  { to: '/', label: 'Home', icon: HomeIcon },
  { to: '/subscriptions', label: 'Subscriptions', icon: BellIcon, requiresAuth: true },
  { to: '/liked', label: 'Liked videos', icon: ThumbIcon, requiresAuth: true },
  { to: '/history', label: 'History', icon: HistoryIcon, requiresAuth: true },
  { to: '/studio', label: 'Studio', icon: StudioIcon, requiresAuth: true },
  { to: '/admin', label: 'Admin panel', icon: ShieldIcon, requiresAdmin: true },
];

interface SidebarProps {
  open: boolean;
  onClose: () => void;
}

export function Sidebar({ open, onClose }: SidebarProps) {
  const currentUser = useAuthStore((s) => s.currentUser);

  const visibleItems = navItems.filter((item) => {
    if (item.requiresAdmin) return currentUser?.role === 'Admin' || currentUser?.role === 'Moderator';
    if (item.requiresAuth) return Boolean(currentUser);
    return true;
  });

  const content = (
    <nav className="flex flex-col gap-1 p-3">
      {visibleItems.map(({ to, label, icon: Icon }) => (
        <NavLink
          key={to}
          to={to}
          end={to === '/'}
          onClick={onClose}
          className={({ isActive }) =>
            `flex items-center gap-3 rounded-card px-3 py-2 text-sm transition-colors ${
              isActive ? 'bg-surface-raised text-signal' : 'text-paper-dim hover:bg-surface hover:text-paper'
            }`
          }
        >
          <Icon />
          {label}
        </NavLink>
      ))}
    </nav>
  );

  return (
    <>
      <aside className="sticky top-16 hidden h-[calc(100vh-4rem)] w-56 shrink-0 border-r border-border lg:block">
        {content}
      </aside>

      {/* Always mounted (not conditional on `open`) so both the open and
          close transitions can actually play -- conditionally rendering
          only on `open` would pop the drawer in/out instantly with no
          animation on the way out. */}
      <div
        className={`fixed inset-0 z-30 lg:hidden ${open ? '' : 'pointer-events-none'}`}
        aria-hidden={!open}
      >
        <div
          className={`absolute inset-0 bg-black/60 transition-opacity duration-200 ease-smooth ${
            open ? 'opacity-100' : 'opacity-0'
          }`}
          onClick={onClose}
        />
        <aside
          className={`absolute left-0 top-0 h-full w-64 border-r border-border bg-ink transition-transform duration-200 ease-smooth ${
            open ? 'translate-x-0' : '-translate-x-full'
          }`}
        >
          <div className="flex h-16 items-center px-4 font-display text-lg font-semibold">
            i<span className="text-signal">Tube</span>
          </div>
          {content}
        </aside>
      </div>
    </>
  );
}
