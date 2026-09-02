import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { AuthShell, FormField, SubmitButton } from '@/components/layout/AuthShell';
import { useRegister } from '@/hooks/useAuth';

export function RegisterPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const register = useRegister();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    register.mutate({ email, password });
  }

  return (
    <AuthShell title="Create your account" subtitle="You'll confirm your email on the next step.">
      <form onSubmit={handleSubmit} className="space-y-4">
        <FormField
          label="Email"
          type="email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          autoComplete="email"
        />
        <FormField
          label="Password"
          type="password"
          required
          minLength={8}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="new-password"
        />
        <SubmitButton loading={register.isPending}>Create account</SubmitButton>
      </form>
      <p className="mt-5 text-center text-sm text-paper-dim">
        Already have an account?{' '}
        <Link to="/login" className="text-signal hover:text-signal-hover">
          Sign in
        </Link>
      </p>
    </AuthShell>
  );
}
