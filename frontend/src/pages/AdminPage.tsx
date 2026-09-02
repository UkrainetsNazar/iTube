import { useState } from 'react';
import { useAuthStore } from '@/store/authStore';
import {
  useAdminUserBrowse,
  useBanUser,
  useChangeRole,
  useFindByEmail,
  useUnbanUser,
  useUserDetail,
} from '@/hooks/useAdmin';
import { Pagination } from '@/components/common/Pagination';
import { RowsSkeleton } from '@/components/common/Skeletons';
import { formatCount, formatRelativeDate } from '@/lib/format';
import type { Role } from '@/types';

const PAGE_SIZE = 20;

export function AdminPage() {
  const currentUser = useAuthStore((s) => s.currentUser);
  const isAdmin = currentUser?.role === 'Admin';

  const [channelNameFilter, setChannelNameFilter] = useState('');
  const [page, setPage] = useState(1);
  const [emailQuery, setEmailQuery] = useState('');
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  const { data: browsePage, isLoading: browseLoading } = useAdminUserBrowse(channelNameFilter, page, PAGE_SIZE);
  const findByEmail = useFindByEmail();

  function handleEmailLookup() {
    if (!emailQuery.trim()) return;
    findByEmail.mutate(emailQuery.trim(), {
      onSuccess: (result) => setSelectedUserId(result.id),
    });
  }

  return (
    <div className="grid grid-cols-1 gap-8 lg:grid-cols-[minmax(0,1fr)_380px]">
      <div className="min-w-0">
        <h1 className="mb-5 font-display text-lg font-semibold text-paper">Admin panel</h1>

        {isAdmin && (
          <div className="mb-6 flex gap-2">
            <input
              value={emailQuery}
              onChange={(e) => setEmailQuery(e.target.value)}
              placeholder="Look up a user by email"
              className="flex-1 rounded-card border border-border bg-surface px-3 py-2 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
            />
            <button
              onClick={handleEmailLookup}
              disabled={findByEmail.isPending}
              className="rounded-card bg-signal px-4 py-2 text-sm font-medium text-ink hover:bg-signal-hover disabled:opacity-50"
            >
              Look up
            </button>
          </div>
        )}

        <input
          value={channelNameFilter}
          onChange={(e) => {
            setChannelNameFilter(e.target.value);
            setPage(1);
          }}
          placeholder="Filter by channel name"
          className="mb-4 w-full rounded-card border border-border bg-surface px-3 py-2 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
        />

        {browseLoading ? (
          <RowsSkeleton />
        ) : !browsePage || browsePage.items.length === 0 ? (
          <p className="py-12 text-center text-sm text-paper-dim">No users found.</p>
        ) : (
          <div className="overflow-x-auto rounded-card border border-border">
            <table className="w-full min-w-[640px] text-left text-sm">
              <thead className="border-b border-border text-xs uppercase tracking-wide text-paper-faint">
                <tr>
                  <th className="px-4 py-3 font-medium">Channel</th>
                  <th className="px-4 py-3 font-medium">Role</th>
                  <th className="px-4 py-3 font-medium">Status</th>
                  <th className="px-4 py-3 font-medium">Subscribers</th>
                  <th className="px-4 py-3 font-medium" />
                </tr>
              </thead>
              <tbody>
                {browsePage.items.map((row) => (
                  <tr key={row.userId} className="border-b border-border last:border-0">
                    <td className="px-4 py-3 text-paper">{row.channelName || '—'}</td>
                    <td className="px-4 py-3 text-paper-dim">{row.role}</td>
                    <td className="px-4 py-3">
                      {row.isBanned ? (
                        <span className="rounded-full bg-danger/15 px-2.5 py-1 text-xs text-danger">Banned</span>
                      ) : (
                        <span className="rounded-full bg-moss/15 px-2.5 py-1 text-xs text-moss">Active</span>
                      )}
                    </td>
                    <td className="px-4 py-3 text-paper-dim">{formatCount(row.subscribersCount)}</td>
                    <td className="px-4 py-3 text-right">
                      <button
                        onClick={() => setSelectedUserId(row.userId)}
                        className="text-xs text-signal hover:text-signal-hover"
                      >
                        View
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        <Pagination
          page={page}
          pageSize={PAGE_SIZE}
          itemCount={browsePage?.items.length ?? 0}
          totalCount={browsePage?.totalCount}
          onPageChange={setPage}
        />
      </div>

      <aside>
        {selectedUserId ? (
          <UserDetailPanel userId={selectedUserId} isAdmin={isAdmin} onClose={() => setSelectedUserId(null)} />
        ) : (
          <p className="rounded-card border border-dashed border-border p-6 text-center text-sm text-paper-dim">
            Select a user to view details and take moderation actions.
          </p>
        )}
      </aside>
    </div>
  );
}

function UserDetailPanel({ userId, isAdmin, onClose }: { userId: string; isAdmin: boolean; onClose: () => void }) {
  const { data: user, isLoading } = useUserDetail(userId);
  const changeRole = useChangeRole();
  const banUser = useBanUser();
  const unbanUser = useUnbanUser();

  const [reason, setReason] = useState('');
  const [expiresAt, setExpiresAt] = useState('');

  if (isLoading) return <div className="h-64 animate-pulse rounded-card bg-surface-raised" />;
  if (!user) return <p className="text-sm text-paper-dim">User not found.</p>;

  function handleBan() {
    if (!reason.trim()) return;
    banUser.mutate({ userId, reason: reason.trim(), expiresAt: expiresAt || undefined });
    setReason('');
    setExpiresAt('');
  }

  return (
    <div className="rounded-card border border-border bg-surface p-5">
      <div className="mb-4 flex items-start justify-between">
        <div>
          <h3 className="font-display text-base font-semibold text-paper">{user.channel?.name ?? 'Unnamed channel'}</h3>
          <p className="text-xs text-paper-faint">{user.id}</p>
        </div>
        <button onClick={onClose} className="text-paper-dim hover:text-paper" aria-label="Close">
          ×
        </button>
      </div>

      <dl className="mb-4 space-y-1.5 text-sm">
        <div className="flex justify-between">
          <dt className="text-paper-dim">Role</dt>
          <dd className="text-paper">{user.role}</dd>
        </div>
        <div className="flex justify-between">
          <dt className="text-paper-dim">Status</dt>
          <dd className="text-paper">{user.status}</dd>
        </div>
        <div className="flex justify-between">
          <dt className="text-paper-dim">Joined</dt>
          <dd className="text-paper">{formatRelativeDate(user.createdAt)}</dd>
        </div>
        {user.channel && (
          <div className="flex justify-between">
            <dt className="text-paper-dim">Subscribers</dt>
            <dd className="text-paper">{formatCount(user.channel.subscribersCount)}</dd>
          </div>
        )}
      </dl>

      {isAdmin && (
        <div className="mb-5">
          <label className="block text-xs text-paper-dim">Change role</label>
          <select
            value={user.role}
            onChange={(e) => changeRole.mutate({ userId, role: e.target.value as Role })}
            className="mt-1 w-full rounded-card border border-border bg-ink px-3 py-2 text-sm text-paper focus:border-signal"
          >
            <option value="User">User</option>
            <option value="Moderator">Moderator</option>
            <option value="Admin">Admin</option>
          </select>
        </div>
      )}

      <div className="mb-5 rounded-card bg-ink p-3">
        <p className="mb-2 text-xs font-medium uppercase tracking-wide text-paper-faint">Current ban</p>
        {user.currentBan?.isActive ? (
          <div className="text-sm">
            <p className="text-paper">{user.currentBan.reason}</p>
            <p className="mt-1 text-xs text-paper-dim">
              {user.currentBan.isPermanent
                ? 'Permanent'
                : user.currentBan.expiresAt
                  ? `Expires ${new Date(user.currentBan.expiresAt).toLocaleDateString()}`
                  : 'No expiry set'}
            </p>
            <button
              onClick={() => unbanUser.mutate(userId)}
              disabled={unbanUser.isPending}
              className="mt-2 rounded-card border border-border px-3 py-1.5 text-xs text-paper hover:border-moss hover:text-moss disabled:opacity-50"
            >
              Unban
            </button>
          </div>
        ) : (
          <div className="space-y-2">
            <textarea
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              placeholder="Ban reason"
              rows={2}
              className="w-full resize-none rounded-card border border-border bg-surface px-3 py-2 text-sm text-paper placeholder:text-paper-faint focus:border-signal"
            />
            <label className="block text-xs text-paper-dim">
              Expires (optional, leave blank for permanent)
              <input
                type="date"
                value={expiresAt}
                onChange={(e) => setExpiresAt(e.target.value)}
                className="mt-1 w-full rounded-card border border-border bg-surface px-3 py-2 text-sm text-paper focus:border-signal"
              />
            </label>
            <button
              onClick={handleBan}
              disabled={!reason.trim() || banUser.isPending}
              className="w-full rounded-card bg-danger px-3 py-2 text-sm font-medium text-ink hover:bg-danger-hover disabled:opacity-50"
            >
              Ban user
            </button>
          </div>
        )}
      </div>

      {user.banHistory.length > 0 && (
        <div>
          <p className="mb-2 text-xs font-medium uppercase tracking-wide text-paper-faint">Ban history</p>
          <ul className="space-y-2">
            {user.banHistory.map((b) => (
              <li key={b.id} className="rounded-card bg-ink p-2.5 text-xs text-paper-dim">
                <span className="text-paper">{b.reason}</span> · {formatRelativeDate(b.bannedAt)}
                {b.isActive && <span className="ml-1 text-danger">(active)</span>}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
