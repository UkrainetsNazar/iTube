import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { Spinner } from './Skeletons';

function CenteredSpinner() {
  return (
    <div className="flex h-[60vh] items-center justify-center">
      <Spinner />
    </div>
  );
}

/** Redirects to /login (preserving the intended destination) when logged out. */
export function RequireAuth() {
  const currentUser = useAuthStore((s) => s.currentUser);
  const isInitializing = useAuthStore((s) => s.isInitializing);
  const location = useLocation();

  if (isInitializing) return <CenteredSpinner />;
  if (!currentUser) return <Navigate to="/login" state={{ from: location.pathname }} replace />;
  return <Outlet />;
}

/**
 * Redirects away from /admin for anyone who isn't Admin or Moderator.
 * Admin-only actions within the page (role changes, email lookup) are
 * gated separately inside AdminPage -- see the endpoint table.
 */
export function RequireAdmin() {
  const currentUser = useAuthStore((s) => s.currentUser);
  const isInitializing = useAuthStore((s) => s.isInitializing);

  if (isInitializing) return <CenteredSpinner />;
  if (!currentUser || (currentUser.role !== 'Admin' && currentUser.role !== 'Moderator')) {
    return <Navigate to="/" replace />;
  }
  return <Outlet />;
}
