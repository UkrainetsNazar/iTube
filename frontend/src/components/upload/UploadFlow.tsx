import { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { videosApi } from '@/api/videos';
import { mediaApi } from '@/api/media';
import { TagInput } from './TagInput';
import { useToast } from '@/components/common/ToastProvider';
import { extractApiErrorMessage } from '@/api/client';
import type { MediaProcessingStatus, VideoDto, Visibility } from '@/types';

type Step = 'form' | 'uploading' | 'checking-status' | 'awaiting-sources' | 'ready' | 'publishing' | 'failed';

interface UploadFlowProps {
  onClose: () => void;
}

const STATUS_POLL_INTERVAL_MS = 2000;
const SOURCES_POLL_INTERVAL_MS = 4000;

const statusMessages: Record<MediaProcessingStatus, string> = {
  Pending: 'Queued for processing…',
  Processing: 'Processing your video…',
  Completed: 'Processing complete.',
  Failed: 'Processing failed.',
};

export function UploadFlow({ onClose }: UploadFlowProps) {
  const [step, setStep] = useState<Step>('form');
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [tags, setTags] = useState<string[]>([]);
  const [file, setFile] = useState<File | null>(null);
  const [progress, setProgress] = useState(0);
  const [videoId, setVideoId] = useState<string | null>(null);
  const [mediaAssetId, setMediaAssetId] = useState<string | null>(null);
  const [processingStatus, setProcessingStatus] = useState<MediaProcessingStatus>('Pending');
  const [failureReason, setFailureReason] = useState<string | null>(null);
  const [visibility, setVisibility] = useState<Visibility>('Public');
  const statusPollRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const sourcesPollRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const { showToast } = useToast();
  const queryClient = useQueryClient();

  useEffect(() => {
    return () => {
      if (statusPollRef.current) clearInterval(statusPollRef.current);
      if (sourcesPollRef.current) clearInterval(sourcesPollRef.current);
    };
  }, []);

  const createVideo = useMutation({
    mutationFn: () => videosApi.upload(title.trim(), description.trim(), tags),
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not start the upload.')),
  });

  const uploadMedia = useMutation({
    mutationFn: ({ id, file }: { id: string; file: File }) =>
      mediaApi.upload(file, 'RawVideo', { videoId: id }, setProgress),
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not upload the video file.')),
  });

  const publish = useMutation({
    mutationFn: (id: string) => videosApi.publish(id, visibility),
    onSuccess: () => {
      showToast('Video published.', 'success');
      queryClient.invalidateQueries({ queryKey: ['videos', 'mine'] });
      onClose();
    },
    onError: (err) => showToast(extractApiErrorMessage(err, 'Could not publish. Try again in a moment.')),
  });

  async function handleSubmit() {
    if (!title.trim() || !file) return;
    try {
      const { videoId: newId } = await createVideo.mutateAsync();
      setVideoId(newId);
      setStep('uploading');
      await runMediaUpload(newId, file);
    } catch {
    }
  }

  async function runMediaUpload(id: string, file: File) {
    setProgress(0);
    setFailureReason(null);
    try {
      const result = await uploadMedia.mutateAsync({ id, file });
      setMediaAssetId(result.mediaAssetId);
      setStep('checking-status');
      startStatusPolling(result.mediaAssetId, id);
    } catch {
      setFailureReason('Could not upload the file. Check your connection and try again.');
      setStep('failed');
    }
  }

  function startStatusPolling(assetId: string, id: string) {
    setProcessingStatus('Pending');
    statusPollRef.current = setInterval(async () => {
      try {
        const { status, failureReason: reason } = await mediaApi.checkStatus(assetId);
        setProcessingStatus(status);

        if (status === 'Completed') {
          if (statusPollRef.current) clearInterval(statusPollRef.current);
          setStep('awaiting-sources');
          startSourcesPolling(id);
        } else if (status === 'Failed') {
          if (statusPollRef.current) clearInterval(statusPollRef.current);
          setFailureReason(reason ?? 'Processing failed for an unknown reason.');
          setStep('failed');
        }
      } catch {
      }
    }, STATUS_POLL_INTERVAL_MS);
  }

  function startSourcesPolling(id: string) {
    sourcesPollRef.current = setInterval(async () => {
      const v: VideoDto = await videosApi.getById(id);
      if (v.sources.length > 0) {
        if (sourcesPollRef.current) clearInterval(sourcesPollRef.current);
        setStep('ready');
      }
    }, SOURCES_POLL_INTERVAL_MS);
  }

  function handlePublish() {
    if (!videoId) return;
    setStep('publishing');
    publish.mutate(videoId);
  }

  function handleRetry(newFile: File) {
    if (!videoId) return;
    setFile(newFile);
    setStep('uploading');
    runMediaUpload(videoId, newFile);
  }

  function handleCloseMidway() {
    if (statusPollRef.current) clearInterval(statusPollRef.current);
    if (sourcesPollRef.current) clearInterval(sourcesPollRef.current);
    if (videoId) {
      showToast('Your video is saved as a draft in Studio -- come back anytime to finish publishing.', 'info');
    }
    onClose();
  }

      return createPortal(
    <div className="fixed inset-0 z-50 flex animate-fade-in items-center justify-center bg-black/85 p-4 backdrop-blur-sm">
      <div className="flex max-h-[90vh] w-full max-w-lg animate-scale-in flex-col rounded-card border border-border bg-surface shadow-2xl">
        <div className="flex items-center justify-between border-b border-border px-6 py-4">
          <h2 className="font-display text-lg font-semibold text-paper">Upload video</h2>
          <button
            onClick={handleCloseMidway}
            className="flex h-8 w-8 items-center justify-center rounded-full text-lg text-paper-dim transition-colors hover:bg-surface-raised hover:text-paper"
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <div className="overflow-y-auto p-6">
        <div key={step} className="animate-fade-in-up">
        {step === 'form' && (
          <div className="space-y-4">
            <label className="block text-sm">
              <span className="mb-1.5 block text-paper-dim">Title</span>
              <input
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                className="w-full rounded-card border border-border bg-ink px-3 py-2 text-paper focus:border-signal"
                placeholder="Give your video a title"
              />
            </label>
            <label className="block text-sm">
              <span className="mb-1.5 block text-paper-dim">Description</span>
              <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                rows={3}
                className="w-full resize-none rounded-card border border-border bg-ink px-3 py-2 text-paper focus:border-signal"
                placeholder="Tell viewers about your video"
              />
            </label>
            <label className="block text-sm">
              <span className="mb-1.5 block text-paper-dim">Tags</span>
              <TagInput tags={tags} onChange={setTags} />
            </label>
            <label className="block text-sm">
              <span className="mb-1.5 block text-paper-dim">Video file</span>
              <input
                type="file"
                accept="video/*"
                onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                className="block w-full text-sm text-paper-dim file:mr-3 file:rounded-card file:border-0 file:bg-surface-raised file:px-3 file:py-2 file:text-paper hover:file:bg-surface-hover"
              />
            </label>
            <button
              onClick={handleSubmit}
              disabled={!title.trim() || !file || createVideo.isPending}
              className="w-full rounded-card bg-signal px-4 py-2.5 text-sm font-medium text-ink hover:bg-signal-hover disabled:opacity-50"
            >
              {createVideo.isPending ? 'Starting…' : 'Start upload'}
            </button>
          </div>
        )}

        {step === 'uploading' && (
          <div className="py-6 text-center">
            <p className="mb-3 text-sm text-paper-dim">Uploading your video file…</p>
            <div className="h-2 w-full overflow-hidden rounded-full bg-surface-raised">
              <div className="h-full bg-signal transition-all" style={{ width: `${progress}%` }} />
            </div>
            <p className="mt-2 text-xs text-paper-faint">{progress}%</p>
          </div>
        )}

        {(step === 'checking-status' || step === 'awaiting-sources') && (
          <div className="py-6 text-center">
            <div className="mx-auto mb-3 h-6 w-6 animate-spin rounded-full border-2 border-border border-t-signal" />
            <p className="text-sm text-paper">
              {step === 'awaiting-sources' ? statusMessages.Completed : statusMessages[processingStatus]}
            </p>
            <p className="mt-2 text-xs text-paper-faint">
              You can close this and check back in Studio; your draft is saved.
            </p>
          </div>
        )}

        {step === 'failed' && (
          <FailedStep reason={failureReason} onRetry={handleRetry} />
        )}

        {step === 'ready' && (
          <div className="space-y-4">
            <p className="text-sm text-paper">Processing complete. Choose who can see this video, then publish.</p>
            <label className="block text-sm">
              <span className="mb-1.5 block text-paper-dim">Visibility</span>
              <select
                value={visibility}
                onChange={(e) => setVisibility(e.target.value as Visibility)}
                className="w-full rounded-card border border-border bg-ink px-3 py-2 text-paper focus:border-signal"
              >
                <option value="Public">Public</option>
                <option value="Private">Private</option>
              </select>
            </label>
            <button
              onClick={handlePublish}
              className="w-full rounded-card bg-signal px-4 py-2.5 text-sm font-medium text-ink hover:bg-signal-hover"
            >
              Publish
            </button>
          </div>
        )}

        {step === 'publishing' && <p className="py-6 text-center text-sm text-paper-dim">Publishing…</p>}
        </div>
        </div>
      </div>
    </div>,
    document.body
  );
}

function FailedStep({ reason, onRetry }: { reason: string | null; onRetry: (file: File) => void }) {
  const inputRef = useRef<HTMLInputElement>(null);

  return (
    <div className="space-y-4">
      <div className="rounded-card border border-danger/40 bg-danger/10 p-4">
        <p className="text-sm font-medium text-paper">Processing failed</p>
        <p className="mt-1 text-sm text-paper-dim">{reason ?? 'An unknown error occurred while processing your video.'}</p>
      </div>
      <p className="text-sm text-paper-dim">
        Your draft is still saved -- pick a file to try uploading again, or close this and retry later from Studio.
      </p>
      <input
        ref={inputRef}
        type="file"
        accept="video/*"
        onChange={(e) => e.target.files?.[0] && onRetry(e.target.files[0])}
        className="block w-full text-sm text-paper-dim file:mr-3 file:rounded-card file:border-0 file:bg-surface-raised file:px-3 file:py-2 file:text-paper hover:file:bg-surface-hover"
      />
    </div>
  );
}