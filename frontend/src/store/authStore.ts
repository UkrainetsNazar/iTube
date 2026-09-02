import { create } from 'zustand';
import type { CurrentUser } from '@/types';

interface AuthState {
  currentUser: CurrentUser | null;
  /** True while /auth/me is being resolved on initial app load. */
  isInitializing: boolean;
  setCurrentUser: (user: CurrentUser | null) => void;
  setInitializing: (value: boolean) => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  currentUser: null,
  isInitializing: true,
  setCurrentUser: (user) => set({ currentUser: user }),
  setInitializing: (value) => set({ isInitializing: value }),
}));
