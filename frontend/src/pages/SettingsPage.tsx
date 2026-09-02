import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { useChangePassword } from '@/hooks/useAuth';

export function SettingsPage() {
  const currentUser = useAuthStore((s) => s.currentUser);
  const changePassword = useChangePassword();

  return (
    <div className="mx-auto max-w-2xl space-y-10">
      <h1 className="font-display text-lg font-semibold text-paper">Account settings</h1>

      <p className="text-sm text-paper-dim">
        Looking to edit your avatar, banner, name, or description?{' '}
        {currentUser && (
          <Link to={`/channel/${currentUser.id}`} className="text-signal hover:text-signal-hover">
            Edit your channel profile
          </Link>
        )}{' '}
        — hover over your avatar/banner or the pencil icons next to your name and description there.
      </p>

      <section>
        <h2 className="mb-4 font-display text-base font-semibold text-paper">Password</h2>
        <ChangePasswordForm
          onSubmit={(oldPassword, newPassword) => changePassword.mutate({ oldPassword, newPassword })}
          loading={changePassword.isPending}
        />
      </section>
    </div>
  );
}

function ChangePasswordForm({
  onSubmit,
  loading,
}: {
  onSubmit: (oldPassword: string, newPassword: string) => void;
  loading: boolean;
}) {
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    onSubmit(oldPassword, newPassword);
    setOldPassword('');
    setNewPassword('');
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <label className="block text-sm">
        <span className="mb-1.5 block text-paper-dim">Current password</span>
        <input
          type="password"
          required
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
          className="w-full rounded-card border border-border bg-surface px-3 py-2 text-paper focus:border-signal"
        />
      </label>
      <label className="block text-sm">
        <span className="mb-1.5 block text-paper-dim">New password</span>
        <input
          type="password"
          required
          minLength={8}
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          className="w-full rounded-card border border-border bg-surface px-3 py-2 text-paper focus:border-signal"
        />
      </label>
      <button
        type="submit"
        disabled={loading}
        className="rounded-card border border-border px-4 py-2.5 text-sm font-medium text-paper hover:border-signal disabled:opacity-50"
      >
        Update password
      </button>
    </form>
  );
}