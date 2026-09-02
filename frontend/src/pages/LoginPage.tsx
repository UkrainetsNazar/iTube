import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { AuthShell, FormField, SubmitButton } from '@/components/layout/AuthShell';
import { useLogin } from '@/hooks/useAuth';

export function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const login = useLogin();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    login.mutate({ email, password });
  }

  return (
    <AuthShell title="Sign in" subtitle="Welcome back to iTube.">
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
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="current-password"
        />
        <div className="text-right text-xs">
          <Link to="/forgot-password" className="text-paper-dim hover:text-signal">
            Forgot password?
          </Link>
        </div>
        <SubmitButton loading={login.isPending}>Sign in</SubmitButton>
      </form>
      <p className="mt-5 text-center text-sm text-paper-dim">
        New to iTube?{' '}
        <Link to="/register" className="text-signal hover:text-signal-hover">
          Create an account
        </Link>
      </p>
    </AuthShell>
  );
}
