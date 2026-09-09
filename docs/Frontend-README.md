# iTube Frontend

React + TypeScript + Vite frontend for iTube, talking to the API Gateway at
`http://localhost:5010/api` by default (configurable via `VITE_API_BASE_URL`).

## Stack

- **React Router** for routing
- **TanStack Query** for all server state — no raw `useEffect` fetch chains
- **Zustand** for auth state only (`src/store/authStore.ts`)
- **Axios**, with a request interceptor (bearer token) and a response
  interceptor (401 → `/auth/refresh` → retry once, single-flight so
  concurrent 401s only trigger one refresh call) — see `src/api/client.ts`
- **Tailwind CSS**, dark theme, custom design tokens (see
  `tailwind.config.js`)
- No animation library — all motion is CSS/Tailwind keyframes, see
  [Animations](#animations) below

## Local setup

```bash
npm install
cp .env.example .env
npm run dev
```

Requires the iTube backend (all services + gateway) running separately.

## Docker

The project ships with a `Dockerfile` (multi-stage: `node:20-alpine` builds
the app, `nginx:1.27-alpine` serves the static output) and `nginx.conf`
(SPA fallback routing — client-side routes like `/watch/abc123` don't exist
as files, so nginx falls back to `index.html`).

To run alongside the backend's `docker-compose.yml`, add a `frontend`
service pointing `context` at this folder, and set `FRONTEND_PORT` in the
root `.env` — if it's unset, Docker publishes the container on a random
host port instead of erroring, which is confusing to debug. Vite bakes
`VITE_API_BASE_URL` into the JS bundle at **build** time, not container
start time, and it must be a URL the *browser* can reach (e.g.
`http://localhost:5010/api`), not an internal Docker service name like
`http://api-gateway:8080` — the browser makes the API calls, not the
frontend container.

## Project structure

```
src/
  api/         one file per backend service, typed request/response shapes
  types/       shared TS types, mirroring the API
  store/       Zustand auth store
  hooks/       TanStack Query hooks, one file per domain
  components/
    layout/    Header, Sidebar, Layout, AuthShell, SearchBar
    common/    Pagination, Toast, ConfirmDialog, ChannelAvatar, RouteGuards,
               Skeletons, EmptyState, PageTransition
    video/     VideoCard, VideoGrid, VideoPlayer, CommentsSection,
               ReactionButtons, SubscribeButton
    upload/    UploadFlow (multi-step modal), TagInput
  pages/       one component per route
```

## Architectural patterns worth knowing before editing

### Modals must use `createPortal`

Every page is wrapped in `PageTransition` (`src/components/common/PageTransition.tsx`),
which applies a CSS `transform` (via the `animate-fade-in-up` keyframe) for
the page-enter animation. **Any ancestor with a non-`none` `transform`
creates a new containing block for `position: fixed` descendants** — so a
naively-nested `fixed inset-0` modal stops positioning against the real
viewport and instead positions against that transformed page wrapper,
causing exactly the "modal is cut off / stuck near the top / scrolls with
the page" bugs that came up while building `UploadFlow`.

**Fix, and the pattern to follow for any new modal:** render it via
`createPortal(<jsx/>, document.body)`. This puts the modal's DOM node
directly under `<body>`, outside the transformed subtree, while it stays
part of the same React component for state/props/context. Both
`UploadFlow.tsx` and `ConfirmDialog.tsx` do this — copy that pattern for
any future overlay.

### Pagination handles two different response shapes

Some list endpoints return `{ items, totalCount, page, pageSize }`, others
return a bare array with no total count (`/videos/feed`,
`/videos/subscriptions`, `/channels/{id}/videos`). `src/api/videos.ts`
normalizes both into a common `VideoListResult` shape (`totalCount` is
`undefined` for the bare-array endpoints), and
`src/components/common/Pagination.tsx` falls back to a "did this page come
back full" heuristic for Next/Previous when `totalCount` is unknown,
instead of showing a fake page count.

### `GET /recommendations` has no `page` param

The personalized home feed (logged-in users) can only ever show one page —
there's no way to paginate personalized recommendations. `HomePage` hides
pagination controls for logged-in users rather than faking it. See
`useHomeFeed` in `src/hooks/useVideos.ts`.

### Channel avatars are fetched per-ID and cached, not embedded

Comments only carry `authorId` (no name/avatar), and search hits don't
carry author info at all. `src/components/common/ChannelAvatar.tsx` fetches
`GET /channels/{id}` per author; React Query dedupes by `channelId`, so the
same commenter appearing 10 times in a thread only triggers one network
request, not ten. `VideoCard` does the same for the channel name shown
under each video.

### View recording

`POST /videos/{id}/views` takes `{ watchedSeconds }`. `VideoPlayer.tsx`
tracks real accumulated watch time via the `<video>` element's
`timeupdate` event (ignoring backward seeks and large forward jumps so
scrubbing doesn't inflate the count), and fires the request exactly once
per session — either when accumulated watch time crosses 30 seconds, or
when the video ends first (for clips shorter than the threshold). `GET
/videos/{id}` (used to load the watch page) already increments a separate
server-side view buffer, so this must never also fire on page load.

### Upload flow status polling

`UploadFlow.tsx` walks: create video (Draft) → upload raw file → poll
`GET /media/{mediaAssetId}/status` every ~2s (`Pending` → `Processing` →
`Completed`/`Failed`) → once `Completed`, switch to polling
`GET /videos/{id}` until `sources.length > 0` → publish. On `Failed`, it
shows `failureReason` and lets you pick a new file to retry against the
same draft video, without re-entering title/description/tags.

Note: the `GET /media/{mediaAssetId}/status` shape was never in the
original API reference — it's an assumption
(`{ status, failureReason? }`) based on the requested UX. Confirm the real
field/value names against the backend if the polling doesn't behave as
expected.

### Video status now has four values, not two

`VideoDto.status` is `'Uploading' | 'Draft' | 'Published' | 'Failed'`
(backend added `Uploading`/`Failed` after this frontend was originally
built against `Draft | Published` only). `failureReason` is a new field
on `VideoDto`, populated only when `status === 'Failed'`.

Anywhere `VideoDto.status` drives UI (studio/dashboard badges, the
publish button's enabled state) needs to handle all four values, not
just the two it may have been built against originally:
- `Uploading` — show a processing indicator, no publish action available yet
- `Draft` — sources are ready, show the publish action
- `Published` — show the visibility toggle instead of publish
- `Failed` — show `failureReason`, offer delete + re-upload (no retry-in-place)

### Private videos 404, not 403 — and it's deliberate

`GET /videos/{id}` now returns `404 Video.NotFound` for anyone viewing a
Private video who isn't the author — including a bare `404`, never `403`,
so the response doesn't confirm a private video even exists at that id.
Treat a `404` on the watch page as "video not found or not visible to
you," not necessarily "never existed" — don't show a more specific
"this video is private" message, since the backend intentionally doesn't
distinguish those cases either.

Make sure the axios client attaches the bearer token on this request
(should already happen via the existing request interceptor, since it's
unconditional) — without it, an owner viewing their own private video's
link would incorrectly get the same 404 as a stranger.

## Animations

Dependency-free — everything is Tailwind keyframes/utilities (see the
`keyframes`/`animation` block in `tailwind.config.js`) plus plain CSS
transitions. A `prefers-reduced-motion` media query in `src/index.css`
collapses all animation/transition durations to near-zero for anyone with
that OS setting on.

- `PageTransition` fades page content in on route change (enter-only, no
  exit animation for the outgoing page — a deliberate trade-off to avoid
  pulling in a routing-transition library)
- `VideoGrid` staggers card entrance (capped so long grids don't trickle in
  forever); `VideoCard` thumbnails fade in on image load
- `ToastProvider` toasts slide in and properly fade+slide out before
  removal (two-phase: flip a `leaving` flag, then remove after the exit
  transition's duration)
- `UploadFlow` and `ConfirmDialog` fade+scale in; `UploadFlow`'s steps
  crossfade via a `key={step}` remount trick
- `Sidebar`'s mobile drawer and `Header`'s account dropdown are always
  mounted (not conditionally rendered) so both open *and* close transitions
  play, controlled via opacity/transform + `pointer-events-none` when
  closed

## Known gaps against the backend

Some of these were resolved during development (kept here with resolution
noted); others are still open.

| Area | Status | Notes |
|---|---|---|
| `POST /media/upload` response shape | **Resolved** | Actually returns `{ mediaAssetId, status, bucket, key }` — `bucket`/`key` weren't in the original reference doc. Wired into `PATCH /channels/{id}`'s `avatarBucket`/`avatarKey` fields. |
| `PUT` vs `PATCH /channels/{id}` | **Resolved** | Backend uses `PATCH`. `src/api/channels.ts` updated accordingly. |
| `Visibility` values | **Resolved** | `Unlisted` was removed from the backend's contract; the frontend type is now `'Public' \| 'Private'` only. |
| `GET /channels/{id}` exact field names | **Open (best-effort)** | The original reference only named the response type (`ChannelDto`) without listing fields. `name`/`description`/`subscribersCount`/`videoCount` are inferred from the page spec; `avatarUrl`/`bannerUrl` are a guess at the read-side field names. If avatars don't persist across a page reload after upload, this is why — confirm the real response shape. |
| `GET /recommendations` pagination | **Open** | No `page` param exists; see above. |
| Search hits (`VideoSearchHit`) | **Open, by design** | No `authorId`, `sources`, or `visibility` — `VideoCard` renders search results without an author link/avatar as a result. |
| Comments (`CommentDto`) | **Open, by design** | Only `authorId`, no display name — avoided an N+1 name lookup per comment; avatar is fetched (and cached) but no name is shown inline. |
| `GET /media/{mediaAssetId}/status` shape | **Assumed** | Not in the original reference; implemented as `{ status: 'Pending'\|'Processing'\|'Completed'\|'Failed', failureReason?: string }`. |
| Visibility-change search staleness | **Resolved (backend fix, no frontend change needed)** | `PATCH /videos/{id}/visibility` now correctly removes/re-adds the video from the Elasticsearch index. This was previously an open backend gap the frontend had to work around; it no longer needs any client-side handling. |
| Views double-counting | **Resolved** | `POST /videos/{id}/views` now takes `{ watchedSeconds }` and fires once per session based on real accumulated watch time — see [View recording](#view-recording) section above. |
| `VideoDto.status` values | **Open — needs frontend update** | Backend added `Uploading` and `Failed` states (was `Draft`\|`Published` only when this frontend was built). See [Video status now has four values](#video-status-now-has-four-values-not-two) above — any status-driven UI needs auditing against the full four-value set, and `failureReason` (new field) needs surfacing on `Failed`. |
| Private video visibility (`GET /videos/{id}`) | **Open — needs frontend update** | Backend now returns `404` for a Private video viewed by anyone but the owner (previously any video was fetchable by id regardless of visibility). See [Private videos 404](#private-videos-404-not-403--and-its-deliberate) above. |

## Environment variables

| Variable | Where | Purpose |
|---|---|---|
| `VITE_API_BASE_URL` | `.env` (local dev) or Docker build arg | Base URL for all API calls. Baked into the bundle at build time for the Docker image — changing it requires a rebuild, not just a container restart. |
| `FRONTEND_PORT` | Root `.env` (Docker Compose only) | Host port the frontend container is published on. Must be set explicitly or Docker assigns a random port on every `docker compose up`. |

## Troubleshooting notes from development

- **`Cannot find module '@/...'` at build time, but not in the editor** —
  `tsconfig.json`'s `paths` alias is type-checking only; Vite/Rollup needs
  the same alias declared separately in `vite.config.ts`'s `resolve.alias`.
- **`Cannot find module 'node:url'`** — `vite.config.ts` uses Node's
  built-in modules, which needs `@types/node` in `devDependencies`.
