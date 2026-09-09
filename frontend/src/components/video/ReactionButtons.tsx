import type { VideoDto } from '@/types';
import { formatCount } from '@/lib/format';
import { useReactToVideo } from '@/hooks/useVideos';
import { useAuthStore } from '@/store/authStore';
import { useNavigate } from 'react-router-dom';

export function ReactionButtons({ video }: { video: VideoDto }) {
  const currentUser = useAuthStore((s) => s.currentUser);
  const navigate = useNavigate();
  const react = useReactToVideo(video.id);

  function handleReact(type: 'Like' | 'Dislike') {
    if (!currentUser) {
      navigate('/login');
      return;
    }
    // Toggle behavior is server-side: reacting the same way again removes it.
    react.mutate(type);
  }

  return (
    <div className="flex overflow-hidden rounded-card border border-border">
      <button
        onClick={() => handleReact('Like')}
        disabled={react.isPending}
        className="flex items-center gap-2 px-4 py-2 text-sm text-paper transition-all duration-150 hover:bg-surface active:scale-[0.96] disabled:opacity-50"
      >
        <svg width="16" height="16" viewBox="0 0 20 20" fill="none">
          <path
            d="M7 9v8H4V9h3zm0 0l3-6 1 1-1 4h5a1.5 1.5 0 011.4 2l-1.6 5a1.5 1.5 0 01-1.4 1H7"
            stroke="currentColor"
            strokeWidth="1.5"
            strokeLinejoin="round"
          />
        </svg>
        {formatCount(video.likesCount)}
      </button>
      <div className="w-px bg-border" />
      <button
        onClick={() => handleReact('Dislike')}
        disabled={react.isPending}
        className="flex items-center gap-2 px-4 py-2 text-sm text-paper transition-all duration-150 hover:bg-surface active:scale-[0.96] disabled:opacity-50"
      >
        <svg width="16" height="16" viewBox="0 0 20 20" fill="none" style={{ transform: 'rotate(180deg)' }}>
          <path
            d="M7 9v8H4V9h3zm0 0l3-6 1 1-1 4h5a1.5 1.5 0 011.4 2l-1.6 5a1.5 1.5 0 01-1.4 1H7"
            stroke="currentColor"
            strokeWidth="1.5"
            strokeLinejoin="round"
          />
        </svg>
        {formatCount(video.dislikesCount)}
      </button>
    </div>
  );
}
