import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { AuthShell, FormField, SubmitButton } from '@/components/layout/AuthShell';
import { useForgotPassword } from '@/hooks/useAuth';

export function ForgotPasswordPage() {
  const [email, setEmail] = useState('');
  const forgotPassword = useForgotPassword();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    forgotPassword.mutate(email);
  }

  return (
    <AuthShell title="Reset your password" subtitle="We'll email you a reset code.">
      <form onSubmit={handleSubmit} className="space-y-4">
        <FormField label="Email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />
        <SubmitButton loading={forgotPassword.isPending}>Send reset code</SubmitButton>
      </form>
      <p className="mt-5 text-center text-sm text-paper-dim">
        Already have a code?{' '}
        <Link to="/reset-password" className="text-signal hover:text-signal-hover">
          Reset password
        </Link>
      </p>
    </AuthShell>
  );
}
