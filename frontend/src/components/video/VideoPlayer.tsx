import { useMemo, useRef, useState } from 'react';
import type { VideoSource } from '@/types';
import { useRecordView } from '@/hooks/useVideos';

function resolutionRank(res: string): number {
  const n = parseInt(res, 10);
  return Number.isNaN(n) ? 0 : n;
}

const VIEW_THRESHOLD_SECONDS = 30;

export function VideoPlayer({ videoId, sources }: { videoId: string; sources: VideoSource[] }) {
  const sorted = useMemo(() => [...sources].sort((a, b) => resolutionRank(b.resolution) - resolutionRank(a.resolution)), [sources]);
  const [selected, setSelected] = useState(sorted[0]);
  const recordView = useRecordView(videoId);

  const hasRecordedRef = useRef(false);
  const accumulatedRef = useRef(0);
  const lastTimeRef = useRef(0);

  if (sorted.length === 0) {
    return (
      <div className="flex aspect-video w-full items-center justify-center rounded-card bg-surface-raised text-sm text-paper-dim">
        This video has no playable sources yet.
      </div>
    );
  }

  function recordIfDue(finalSeconds?: number) {
    if (hasRecordedRef.current) return;
    const watched = finalSeconds ?? accumulatedRef.current;
    if (watched <= 0) return;
    hasRecordedRef.current = true;
    recordView.mutate(Math.round(watched));
  }

  function handlePlay(e: React.SyntheticEvent<HTMLVideoElement>) {
    lastTimeRef.current = e.currentTarget.currentTime;
  }

  function handleTimeUpdate(e: React.SyntheticEvent<HTMLVideoElement>) {
    const video = e.currentTarget;
    if (video.paused || video.seeking) {
      lastTimeRef.current = video.currentTime;
      return;
    }
    const delta = video.currentTime - lastTimeRef.current;
    if (delta > 0 && delta < 2) {
      accumulatedRef.current += delta;
    }
    lastTimeRef.current = video.currentTime;

    if (accumulatedRef.current >= VIEW_THRESHOLD_SECONDS) {
      recordIfDue();
    }
  }

  function handleEnded(e: React.SyntheticEvent<HTMLVideoElement>) {
    recordIfDue(e.currentTarget.duration || accumulatedRef.current);
  }

  return (
    <div>
      <video
        key={selected.url}
        controls
        className="aspect-video w-full rounded-card bg-black"
        onPlay={handlePlay}
        onTimeUpdate={handleTimeUpdate}
        onEnded={handleEnded}
        src={selected.url}
      />
      {sorted.length > 1 && (
        <div className="mt-2 flex justify-end gap-1.5">
          {sorted.map((s) => (
            <button
              key={s.resolution}
              onClick={() => setSelected(s)}
              className={`rounded-card border px-2.5 py-1 text-xs ${
                s.resolution === selected.resolution
                  ? 'border-signal text-signal'
                  : 'border-border text-paper-dim hover:text-paper'
              }`}
            >
              {s.resolution}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}