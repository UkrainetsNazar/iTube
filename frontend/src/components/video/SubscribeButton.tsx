import { useSubscription } from '@/hooks/useChannel';
import { useAuthStore } from '@/store/authStore';

export function SubscribeButton({ channelId }: { channelId: string }) {
  const currentUser = useAuthStore((s) => s.currentUser);
  const { isSubscribed, isLoading, toggle, isToggling } = useSubscription(channelId);

  // Hidden when logged out -- GET /subscriptions/check and the toggle endpoints both require auth.
  if (!currentUser) return null;
  // Don't let a creator "subscribe" to themselves.
  if (currentUser.id === channelId) return null;

  return (
    <button
      onClick={toggle}
      disabled={isLoading || isToggling}
      className={`rounded-card px-4 py-2 text-sm font-medium transition-all duration-150 active:scale-[0.97] disabled:opacity-50 disabled:active:scale-100 ${
        isSubscribed
          ? 'border border-border text-paper hover:border-danger hover:text-danger'
          : 'bg-signal text-ink hover:bg-signal-hover'
      }`}
    >
      {isSubscribed ? 'Subscribed' : 'Subscribe'}
    </button>
  );
}
