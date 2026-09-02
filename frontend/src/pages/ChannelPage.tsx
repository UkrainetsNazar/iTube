import { useEffect, useRef, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useChannel, useUpdateChannel } from '@/hooks/useChannel';
import { useChannelVideos } from '@/hooks/useVideos';
import { useAuthStore } from '@/store/authStore';
import { mediaApi } from '@/api/media';
import { useToast } from '@/components/common/ToastProvider';
import { SubscribeButton } from '@/components/video/SubscribeButton';
import { VideoGrid } from '@/components/video/VideoGrid';
import { Pagination } from '@/components/common/Pagination';
import { formatCount } from '@/lib/format';

const PAGE_SIZE = 24;

function CameraIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 20 20" fill="none">
      <path
        d="M4 7h2l1-2h6l1 2h2a1 1 0 011 1v7a1 1 0 01-1 1H4a1 1 0 01-1-1V8a1 1 0 011-1z"
        stroke="currentColor"
        strokeWidth="1.5"
        strokeLinejoin="round"
      />
      <circle cx="10" cy="11" r="2.5" stroke="currentColor" strokeWidth="1.5" />
    </svg>
  );
}

function PencilIcon() {
  return (
    <svg width="14" height="14" viewBox="0 0 20 20" fill="none">
      <path
        d="M14 3l3 3-9.5 9.5L4 16l.5-3.5L14 3z"
        stroke="currentColor"
        strokeWidth="1.4"
        strokeLinejoin="round"
        strokeLinecap="round"
      />
    </svg>
  );
}

export function ChannelPage() {
  const { channelId } = useParams<{ channelId: string }>();
  const currentUser = useAuthStore((s) => s.currentUser);
  const isOwnChannel = Boolean(currentUser && channelId && currentUser.id === channelId);

  const { data: channel, isLoading: channelLoading } = useChannel(channelId);
  const [page, setPage] = useState(1);
  const { data, isLoading: videosLoading } = useChannelVideos(channelId, page, PAGE_SIZE);
  const updateChannel = useUpdateChannel(channelId ?? '');
  const { showToast } = useToast();

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [avatarPreview, setAvatarPreview] = useState<string | null>(null);
  const [bannerPreview, setBannerPreview] = useState<string | null>(null);

  const [editingName, setEditingName] = useState(false);
  const [editingDescription, setEditingDescription] = useState(false);
  const [nameDraft, setNameDraft] = useState('');
  const [descriptionDraft, setDescriptionDraft] = useState('');
  const [avatarUploading, setAvatarUploading] = useState(false);
  const [bannerUploading, setBannerUploading] = useState(false);

  const avatarInputRef = useRef<HTMLInputElement>(null);
  const bannerInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (channel) {
      setName(channel.name);
      setDescription(channel.description ?? '');
      if (channel.avatarUrl) setAvatarPreview(channel.avatarUrl);
      if (channel.bannerUrl) setBannerPreview(channel.bannerUrl);
    }
  }, [channel]);

  async function handleImageUpload(file: File, kind: 'Avatar' | 'Banner') {
    if (!channelId) return;
    const setUploading = kind === 'Avatar' ? setAvatarUploading : setBannerUploading;
    const setPreview = kind === 'Avatar' ? setAvatarPreview : setBannerPreview;
    setUploading(true);
    try {
      const result = await mediaApi.upload(file, kind, { channelId });
      setPreview(URL.createObjectURL(file));
      updateChannel.mutate(
        kind === 'Avatar'
          ? { name, description, avatarBucket: result.bucket, avatarKey: result.key }
          : { name, description, bannerBucket: result.bucket, bannerKey: result.key }
      );
    } catch {
      showToast(`Could not upload ${kind.toLowerCase()}.`);
    } finally {
      setUploading(false);
    }
  }

  function startEditingName() {
    setNameDraft(name);
    setEditingName(true);
  }
  function saveName() {
    const trimmed = nameDraft.trim();
    if (!trimmed) return;
    setName(trimmed);
    setEditingName(false);
    updateChannel.mutate({ name: trimmed, description });
  }

  function startEditingDescription() {
    setDescriptionDraft(description);
    setEditingDescription(true);
  }
  function saveDescription() {
    setDescription(descriptionDraft);
    setEditingDescription(false);
    updateChannel.mutate({ name, description: descriptionDraft });
  }

  if (channelLoading) {
    return <div className="h-40 animate-pulse rounded-card bg-surface-raised" />;
  }

  if (!channel) {
    return <p className="py-24 text-center text-paper-dim">This channel couldn't be found.</p>;
  }

  return (
    <div>
      <div className="overflow-hidden rounded-card border border-border">
        {/* Banner */}
        <div className="group relative h-32 bg-surface-raised sm:h-44">
          {bannerPreview && <img src={bannerPreview} alt="" className="h-full w-full object-cover" />}
          {isOwnChannel && (
            <>
              <button
                onClick={() => bannerInputRef.current?.click()}
                disabled={bannerUploading}
                className="absolute bottom-2 right-2 flex items-center gap-1.5 rounded-card bg-black/60 px-3 py-1.5 text-xs text-paper opacity-0 backdrop-blur transition-opacity hover:bg-black/80 group-hover:opacity-100 disabled:opacity-50"
              >
                <CameraIcon />
                {bannerUploading ? 'Uploading…' : 'Change banner'}
              </button>
              <input
                ref={bannerInputRef}
                type="file"
                accept="image/*"
                className="hidden"
                onChange={(e) => e.target.files?.[0] && handleImageUpload(e.target.files[0], 'Banner')}
              />
            </>
          )}
        </div>

        <div className="flex flex-col gap-4 bg-surface p-5 sm:flex-row sm:items-center">
          {/* Avatar */}
          <div className="group relative -mt-12 h-20 w-20 shrink-0 rounded-full border-4 border-surface bg-surface-raised sm:-mt-16 sm:h-24 sm:w-24">
            {avatarPreview && (
              <img src={avatarPreview} alt="" className="h-full w-full rounded-full object-cover" />
            )}
            {isOwnChannel && (
              <>
                <button
                  onClick={() => avatarInputRef.current?.click()}
                  disabled={avatarUploading}
                  className="absolute inset-0 flex items-center justify-center rounded-full bg-black/50 text-paper opacity-0 transition-opacity hover:bg-black/70 group-hover:opacity-100 disabled:opacity-50"
                  aria-label="Change avatar"
                >
                  <CameraIcon />
                </button>
                <input
                  ref={avatarInputRef}
                  type="file"
                  accept="image/*"
                  className="hidden"
                  onChange={(e) => e.target.files?.[0] && handleImageUpload(e.target.files[0], 'Avatar')}
                />
              </>
            )}
          </div>

          <div className="min-w-0 flex-1">
            {/* Name */}
            {editingName ? (
              <div className="flex items-center gap-2">
                <input
                  autoFocus
                  value={nameDraft}
                  onChange={(e) => setNameDraft(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && saveName()}
                  className="rounded-card border border-border bg-ink px-2.5 py-1 font-display text-xl font-semibold text-paper focus:border-signal"
                />
                <button onClick={saveName} className="text-xs text-signal hover:text-signal-hover">
                  Save
                </button>
                <button onClick={() => setEditingName(false)} className="text-xs text-paper-dim hover:text-paper">
                  Cancel
                </button>
              </div>
            ) : (
              <h1 className="group flex items-center gap-2 font-display text-xl font-semibold text-paper">
                {name}
                {isOwnChannel && (
                  <button
                    onClick={startEditingName}
                    className="text-paper-faint opacity-0 transition-opacity hover:text-signal group-hover:opacity-100"
                    aria-label="Edit channel name"
                  >
                    <PencilIcon />
                  </button>
                )}
              </h1>
            )}

            <p className="text-sm text-paper-dim">
              {formatCount(channel.subscribersCount)} subscribers · {formatCount(channel.videoCount)} videos
            </p>

            {/* Description */}
            {editingDescription ? (
              <div className="mt-2 max-w-2xl">
                <textarea
                  autoFocus
                  value={descriptionDraft}
                  onChange={(e) => setDescriptionDraft(e.target.value)}
                  rows={3}
                  className="w-full resize-none rounded-card border border-border bg-ink px-2.5 py-1.5 text-sm text-paper focus:border-signal"
                />
                <div className="mt-1.5 flex gap-2">
                  <button onClick={saveDescription} className="text-xs text-signal hover:text-signal-hover">
                    Save
                  </button>
                  <button
                    onClick={() => setEditingDescription(false)}
                    className="text-xs text-paper-dim hover:text-paper"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            ) : (
              <div className="group mt-2 flex max-w-2xl items-start gap-2">
                {description ? (
                  <p className="text-sm text-paper">{description}</p>
                ) : isOwnChannel ? (
                  <p className="text-sm text-paper-faint italic">No description yet.</p>
                ) : null}
                {isOwnChannel && (
                  <button
                    onClick={startEditingDescription}
                    className="mt-0.5 shrink-0 text-paper-faint opacity-0 transition-opacity hover:text-signal group-hover:opacity-100"
                    aria-label="Edit description"
                  >
                    <PencilIcon />
                  </button>
                )}
              </div>
            )}
          </div>

          {channelId && !isOwnChannel && <SubscribeButton channelId={channelId} />}
        </div>
      </div>

      <div className="mt-8">
        <VideoGrid videos={data?.items} isLoading={videosLoading} emptyTitle="No public videos yet" />
        {data && data.items.length > 0 && (
          <Pagination
            page={page}
            pageSize={PAGE_SIZE}
            itemCount={data.items.length}
            totalCount={data.totalCount}
            onPageChange={setPage}
          />
        )}
      </div>
    </div>
  );
}