import type { TokenPair } from '@/types';

// SECURITY NOTE: tokens are kept in memory (fast path for the axios
// interceptor) and mirrored into localStorage so a page refresh doesn't
// force a re-login. Storing bearer tokens in localStorage is readable by
// any script on the page, so this is vulnerable to XSS-based token theft.
// That's an accepted tradeoff for this project's scope -- a production
// build should move refresh tokens to an httpOnly cookie set by the
// backend instead.

const ACCESS_KEY = 'itube.accessToken';
const REFRESH_KEY = 'itube.refreshToken';

let accessToken: string | null = localStorage.getItem(ACCESS_KEY);
let refreshToken: string | null = localStorage.getItem(REFRESH_KEY);

export const tokenStorage = {
  getAccessToken(): string | null {
    return accessToken;
  },
  getRefreshToken(): string | null {
    return refreshToken;
  },
  setTokens(tokens: TokenPair) {
    accessToken = tokens.accessToken;
    refreshToken = tokens.refreshToken;
    localStorage.setItem(ACCESS_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_KEY, tokens.refreshToken);
  },
  clear() {
    accessToken = null;
    refreshToken = null;
    localStorage.removeItem(ACCESS_KEY);
    localStorage.removeItem(REFRESH_KEY);
  },
  hasTokens(): boolean {
    return Boolean(accessToken && refreshToken);
  },
};
