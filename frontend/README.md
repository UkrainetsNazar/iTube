# iTube frontend

React + TypeScript + Vite frontend for iTube, built against the API Gateway
at `http://localhost:5010/api` (configurable via `VITE_API_BASE_URL`).

## Setup

```bash
npm install
cp .env.example .env   # adjust VITE_API_BASE_URL if your gateway runs elsewhere
npm run dev
```

Requires the iTube backend (all 5 services + gateway) running separately.

## Stack

- React Router for routing
- TanStack Query for all server state (no raw `useEffect` fetch chains)
- Zustand for auth state (`src/store/authStore.ts`)
- Axios with a request interceptor (bearer token) and response interceptor
  (401 → `/auth/refresh` → retry once, single-flight so concurrent 401s
  only trigger one refresh call) — see `src/api/client.ts`
- Tailwind CSS

## Structure

```
src/
  api/         one file per backend service, typed request/response shapes
  types/       shared TS types, mirroring the API reference
  store/       Zustand auth store
  hooks/       TanStack Query hooks, one file per domain
  components/  layout chrome, shared UI (pagination, toasts, skeletons),
               video-specific components, upload flow
  pages/       one component per route
```

## Known gaps against the API reference

These are flagged in code comments at the relevant spot too, listed here
for visibility:

1. **`ChannelDto` shape is unconfirmed.** The reference names the response
   type for `GET /channels/{id}` but never lists its fields. `name`,
   `description`, `subscribersCount`, `videoCount` are inferred from the
   page spec; `avatarUrl`/`bannerUrl` are a guess at the *read* side and
   don't have documented names. See `src/types/index.ts`.
2. **Avatar/banner upload can't be wired to the channel PUT yet.**
   `POST /media/upload` returns `{ mediaAssetId, status }`, but
   `PUT /channels/{id}` expects `avatarBucket`/`avatarKey` (or
   `bannerBucket`/`bannerKey`). There's no documented mapping from a
   `mediaAssetId` to those two fields. `SettingsPage` uploads the file and
   surfaces the raw response, but does not (yet) send it on to the channel
   update. See the comment above `handleImageUpload` in
   `src/pages/SettingsPage.tsx`.
3. **`GET /recommendations` has no `page` param.** The personalized home
   feed (logged-in users) can only ever show one page of results — there's
   no way to paginate it. `HomePage` hides pagination controls in that
   case rather than faking it. See `src/hooks/useVideos.ts`.
4. **Search results (`VideoSearchHit`) don't include `authorId`, `sources`,
   or `visibility`.** `VideoCard` renders search hits without an author
   link or avatar (no `authorId` to link to), and clicking through to
   `/watch/:id` always issues a fresh `GET /videos/{id}` for full detail
   rather than reusing anything from the search response.
5. **Comments (`CommentDto`) only carry `authorId`, not a display name.**
   `CommentsSection` links each comment's avatar/byline to
   `/channel/{authorId}` rather than showing a name inline, to avoid an
   N+1 `GET /channels/{id}` per comment on every page load.
6. **Visibility-change search staleness** and **views double-counting**
   are handled per the reference's notes (view recording fires once, from
   the `<video>` `play` event, not on page load; visibility changes are
   sent as-is with no client-side attempt to filter stale search results,
   since `hits` doesn't carry `visibility` to filter on anyway).
