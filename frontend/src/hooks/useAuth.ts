import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { authApi } from '@/api/auth';
import { tokenStorage } from '@/api/tokenStorage';
import { useAuthStore } from '@/store/authStore';
import { useToast } from '@/components/common/ToastProvider';
import { extractApiErrorMessage } from '@/api/client';

/** Loads the current user on app boot if tokens already exist. Called once from App.tsx. */
export async function bootstrapAuth() {
  const { setCurrentUser, setInitializing } = useAuthStore.getState();
  if (!tokenStorage.hasTokens()) {
    setInitializing(false);
    return;
  }
  try {
    const user = await authApi.me();
    setCurrentUser(user);
  } catch {
    tokenStorage.clear();
    setCurrentUser(null);
  } finally {
    setInitializing(false);
  }
}

export function useLogin() {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const setCurrentUser = useAuthStore((s) => s.setCurrentUser);

  return useMutation({
    mutationFn: ({ email, password }: { email: string; password: string }) => authApi.login(email, password),
    onSuccess: async (tokens) => {
      tokenStorage.setTokens(tokens);
      const user = await authApi.me();
      setCurrentUser(user);
      navigate('/');
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not sign in.')),
  });
}

export function useRegister() {
  const navigate = useNavigate();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: ({ email, password }: { email: string; password: string }) => authApi.register(email, password),
    onSuccess: (_data, variables) => {
      showToast('Account created. Check your email for a confirmation code.', 'success');
      navigate(`/confirm-email?email=${encodeURIComponent(variables.email)}`);
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not create account.')),
  });
}

export function useConfirmEmail() {
  const navigate = useNavigate();
  const { showToast } = useToast();

  return useMutation({
    mutationFn: ({ email, token }: { email: string; token: string }) => authApi.confirmEmail(email, token),
    onSuccess: () => {
      showToast('Email confirmed. You can sign in now.', 'success');
      navigate('/login');
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not confirm email.')),
  });
}

export function useForgotPassword() {
  const { showToast } = useToast();
  return useMutation({
    mutationFn: (email: string) => authApi.forgotPassword(email),
    onSuccess: () => showToast('If that email exists, a reset code is on its way.', 'success'),
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not send reset email.')),
  });
}

export function useResetPassword() {
  const navigate = useNavigate();
  const { showToast } = useToast();
  return useMutation({
    mutationFn: ({ email, token, newPassword }: { email: string; token: string; newPassword: string }) =>
      authApi.resetPassword(email, token, newPassword),
    onSuccess: () => {
      showToast('Password reset. Sign in with your new password.', 'success');
      navigate('/login');
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not reset password.')),
  });
}

export function useChangePassword() {
  const { showToast } = useToast();
  return useMutation({
    mutationFn: ({ oldPassword, newPassword }: { oldPassword: string; newPassword: string }) =>
      authApi.changePassword(oldPassword, newPassword),
    onSuccess: () => showToast('Password updated.', 'success'),
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not update password.')),
  });
}

export function useLogout() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const setCurrentUser = useAuthStore((s) => s.setCurrentUser);

  return useMutation({
    mutationFn: async () => {
      const refreshToken = tokenStorage.getRefreshToken();
      if (refreshToken) {
        await authApi.logout(refreshToken).catch(() => undefined);
      }
    },
    onSettled: () => {
      tokenStorage.clear();
      setCurrentUser(null);
      queryClient.clear();
      navigate('/');
    },
  });
}
