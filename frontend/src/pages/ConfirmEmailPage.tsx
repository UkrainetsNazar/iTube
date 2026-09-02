import { useState, type FormEvent } from 'react';
import { useSearchParams } from 'react-router-dom';
import { AuthShell, FormField, SubmitButton } from '@/components/layout/AuthShell';
import { useConfirmEmail } from '@/hooks/useAuth';

export function ConfirmEmailPage() {
  const [params] = useSearchParams();
  const [email, setEmail] = useState(params.get('email') ?? '');
  const [token, setToken] = useState('');
  const confirmEmail = useConfirmEmail();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    confirmEmail.mutate({ email, token });
  }

  return (
    <AuthShell title="Confirm your email" subtitle="Paste the confirmation code we emailed you.">
      <form onSubmit={handleSubmit} className="space-y-4">
        <FormField label="Email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />
        <FormField
          label="Confirmation code"
          type="text"
          required
          value={token}
          onChange={(e) => setToken(e.target.value)}
          placeholder="Paste the code from your email"
        />
        <SubmitButton loading={confirmEmail.isPending}>Confirm email</SubmitButton>
      </form>
    </AuthShell>
  );
}
