import { useState, type FormEvent } from 'react';
import { useSearchParams } from 'react-router-dom';
import { AuthShell, FormField, SubmitButton } from '@/components/layout/AuthShell';
import { useResetPassword } from '@/hooks/useAuth';

export function ResetPasswordPage() {
  const [params] = useSearchParams();
  const [email, setEmail] = useState(params.get('email') ?? '');
  const [token, setToken] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const resetPassword = useResetPassword();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    resetPassword.mutate({ email, token, newPassword });
  }

  return (
    <AuthShell title="Set a new password" subtitle="Paste the reset code we emailed you.">
      <form onSubmit={handleSubmit} className="space-y-4">
        <FormField label="Email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />
        <FormField
          label="Reset code"
          type="text"
          required
          value={token}
          onChange={(e) => setToken(e.target.value)}
          placeholder="Paste the code from your email"
        />
        <FormField
          label="New password"
          type="password"
          required
          minLength={8}
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          autoComplete="new-password"
        />
        <SubmitButton loading={resetPassword.isPending}>Reset password</SubmitButton>
      </form>
    </AuthShell>
  );
}
