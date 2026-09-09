import type { ReactNode } from 'react';
import { useLocation } from 'react-router-dom';

/**
 * No page-transition library here (no framer-motion etc.) -- this is a
 * cheap, dependency-free approximation: keying a wrapper div by pathname
 * forces React to remount it on navigation, which re-triggers the CSS
 * entrance animation. It's an enter-only effect (no exit animation for
 * the outgoing page), which is a reasonable trade for zero extra
 * dependencies and no risk of stale/overlapping page content mid-transition.
 */
export function PageTransition({ children }: { children: ReactNode }) {
  const location = useLocation();
  return (
    <div key={location.pathname} className="animate-fade-in-up">
      {children}
    </div>
  );
}
