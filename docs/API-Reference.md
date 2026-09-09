# iTube API Reference

Base URL (via Gateway): `http://localhost:5010/api`

## Conventions
- 🔒 = requires `Authorization: Bearer <accessToken>`. 🔒`Role` = requires that role too.
- Errors: `{ "code": "string", "message": "string" }` — `400`/`404`/`409`/`500`.
- JSON is camelCase. Empty-body success responses are `200 OK` with no content.
- Rate-limited at the Gateway: `login`/`register` — 5/min; `media/upload` — 10/min.
- Max upload size: 2 GB (raw video and image uploads alike).

---

## AuthService — `/api/auth`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| POST | `/register` | — | `{ email, password }` | 200 |
| POST | `/confirm-email` | — | `{ email, token }` | 200 |
| POST | `/login` | — | `{ email, password }` | `{ accessToken, refreshToken }` |
| POST | `/refresh` | — | `{ refreshToken }` | `{ accessToken, refreshToken }` |
| POST | `/logout` | — | `{ refreshToken }` | 200 |
| POST | `/change-password` | 🔒 | `{ oldPassword, newPassword }` | 200 |
| POST | `/forgot-password` | — | `{ email }` | 200 |
| POST | `/reset-password` | — | `{ email, token, newPassword }` | 200 |
| GET | `/me` | 🔒 | — | `{ id, email, role }` |
| GET | `/users/by-email?email=` | 🔒`Admin` | — | `{ id, email }` |

---

## MediaService — `/api/media`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| POST | `/upload` | 🔒 | multipart: `file`, `mediaType` (`RawVideo`\|`Avatar`\|`Banner`), `videoId`? (required if RawVideo), `channelId`? (required if Avatar/Banner) | `{ mediaAssetId, status, bucket, key }` |
| GET | `/{id}/status` | — | — | `{ mediaAssetId, status, failureReason }` |

`status` values: `Pending` → `Processing` → `Completed` \| `Failed`. Avatar/Banner uploads skip processing and return `Completed` immediately.

`bucket`/`key` from the upload response are what you feed into `PATCH /channels/{id}`'s `avatarBucket`/`avatarKey` (or `bannerBucket`/`bannerKey`) fields — UserService constructs the final public URL server-side from these.

**Progress UX**: there's no true percentage progress (FFmpeg's frame-by-frame progress isn't parsed/exposed). Poll `GET /media/{id}/status` after upload and show status-based messaging (Queued → Processing → Ready), not a percentage bar.

---

## UserService

### `/api/channels`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| GET | `/{id}` | — | — | `ChannelDto` |
| PATCH | `/{id}` | 🔒 owner | `{ name, description?, avatarBucket?, avatarKey?, bannerBucket?, bannerKey? }` | 200 |

**`PATCH`, not `PUT`**: this is a genuine partial update — any field omitted from the request body is left **unchanged**, not cleared. There is currently no way to explicitly remove an avatar/banner once set (sending `null`/omitting the field just preserves whatever's already there).

**`ChannelDto`**: `{ id, ownerId, name, description, avatarUrl, bannerUrl, subscribersCount, videoCount, status, createdAt }` — `avatarUrl`/`bannerUrl` are ready-to-use, publicly servable URLs (no auth needed to load the image itself). `videoCount` reflects **published** videos only, kept in sync via events as videos are published/deleted.

### `/api/subscriptions`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| POST | `` | 🔒 | `{ targetChannelId }` | 200 |
| DELETE | `` | 🔒 | `{ targetChannelId }` | 200 |
| GET | `/me` | 🔒 | — | `[{ channelId }]` |
| GET | `/check?channelId=` | 🔒 | — | `true`/`false` |

### `/api/moderation`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| POST | `/users/{id}/ban` | 🔒`Admin,Moderator` | `{ reason, expiresAt? }` (omit/null for permanent) | 200 |
| DELETE | `/users/{id}/ban` | 🔒`Admin,Moderator` | — | 200 |

### `/api/users`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| GET | `/{id}` | — | — | `UserDto` |
| GET | `/users?channelName=&page=&pageSize=` | 🔒`Admin,Moderator` | — | `AdminUserPage` |
| PATCH | `/{id}/role` | 🔒`Admin` | `{ role }` | 200 |

**`UserDto`**:
```json
{
  "id": "guid", "role": "User|Moderator|Admin", "status": "Active|Banned|Deleted",
  "createdAt": "datetime",
  "channel": { "id": "guid", "name": "string", "description": "string|null",
               "subscribersCount": 0, "videoCount": 0, "status": "Active|Banned" } | null,
  "currentBan": { "id": "guid", "bannedByModeratorId": "guid", "reason": "string",
                  "bannedAt": "datetime", "expiresAt": "datetime|null",
                  "isPermanent": true, "isActive": true } | null,
  "banHistory": [ { "id": "guid", "bannedByModeratorId": "guid", "reason": "string",
                     "bannedAt": "datetime", "expiresAt": "datetime|null",
                     "unbannedAt": "datetime|null", "unbannedByModeratorId": "guid|null" } ]
}
```

**`AdminUserPage`** (lighter — list/browse view, no ban history):
```json
{
  "items": [ { "userId": "guid", "channelName": "string", "role": "string", "status": "string",
               "isBanned": false, "banReason": "string|null", "banExpiresAt": "datetime|null",
               "subscribersCount": 0, "createdAt": "datetime" } ],
  "totalCount": 0, "page": 1, "pageSize": 20
}
```

---

## VideoService

### `/api/videos`

| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| POST | `/upload` | 🔒 | `{ title, description?, tags: string[] }` | `{ videoId }` |
| GET | `/{id}` | optional 🔒 | — | `VideoDto` |
| PATCH | `/{id}/publish` | 🔒 owner | `{ visibility: "Public"\|"Private" }` | 200 |
| PATCH | `/{id}/visibility` | 🔒 owner | `{ visibility: "Public"\|"Private" }` | 200 |
| DELETE | `/{id}` | 🔒 owner | — | 200 |
| POST | `/{id}/react` | 🔒 | `{ type: "Like"\|"Dislike" }` | 200 |
| POST | `/{id}/views` | optional 🔒 | `{ watchedSeconds: number }` | 200 |
| GET | `/feed?page=&pageSize=` | — | — | `VideoDto[]` |
| GET | `/subscriptions?page=&pageSize=` | 🔒 | — | `VideoDto[]` |
| GET | `/history?page=&pageSize=` | 🔒 | — | `{ items: VideoDto[], totalCount, page, pageSize }` |
| GET | `/liked?page=&pageSize=` | 🔒 | — | `{ items: VideoDto[], totalCount, page, pageSize }` |
| GET | `/search?q=&tags=&page=&pageSize=` | — | — | `{ hits: [...], totalCount }` |

**`VideoDto`**:
```json
{
  "id": "guid", "title": "string", "description": "string",
  "status": "Uploading|Draft|Published|Failed", "failureReason": "string|null",
  "visibility": "Public|Private",
  "authorId": "guid", "thumbnailUrl": "string|null",
  "viewsCount": 0, "likesCount": 0, "dislikesCount": 0,
  "tags": ["string"],
  "sources": [{ "resolution": "R480p|R720p|R1080p", "url": "string", "format": "mp4" }],
  "createdAt": "datetime", "publishedAt": "datetime|null"
}
```

**Status lifecycle**: `Uploading` (raw file received, FFmpeg processing) → `Draft` (processing succeeded, at least one source attached — publishable) → `Published`. Or, on processing error: `Uploading` → `Failed` (check `failureReason`). A `Failed` video has no automatic retry — delete it and re-upload.

**`DELETE /{id}` is a hard delete** — the row is permanently removed. Works from any status.

**Private videos are fully hidden from everyone except the owner** — not just excluded from feeds/search, but `GET /{id}` itself returns `404 Video.NotFound` (never `403`, deliberately, so a private video's existence isn't revealed) for anyone who isn't the author. Send your auth token on this request even though it's not required, so the owner can still view their own private videos.

**Publish vs. Visibility**: `PATCH /{id}/publish` is for the *first* publish (`Draft` → `Published`; requires `sources.length > 0` and updates `publishedAt`). `PATCH /{id}/visibility` changes visibility on an *already-published* video without touching `publishedAt`, and correctly updates the search index (removes from search on Public→Private, re-indexes on Private→Public) — use this for the "manage my videos" visibility toggle, not `/publish`.

**View tracking (YouTube-style, not TikTok-style)**:
- `GET /{id}` does **not** increment views — loading the page/metadata isn't a watch.
- `POST /{id}/views` is the only way a view is recorded. Call it **once**, when your local playback timer first crosses **30 seconds of watched content**.
- A signed-in viewer replaying the same video won't generate a second view within a **4-hour cooldown**.
- Anonymous viewers have no dedup — every qualifying anonymous playback counts.
- `watchedSeconds` is client-reported and not independently verified server-side.

### `/api/channels/{channelId}/videos`
| Method | Auth | Response |
|---|---|---|
| GET `?page=&pageSize=` | — | `VideoDto[]` (Published + Public only) |

### `/api/users/me/videos`
| Method | Auth | Response |
|---|---|---|
| GET `?page=&pageSize=` | 🔒 | `{ items: VideoDto[], totalCount, page, pageSize }` — **all** statuses/visibilities (creator dashboard) |

### `/api/comments`
| Method | Path | Auth | Body | Response |
|---|---|---|---|---|
| POST | `/api/videos/{videoId}/comments` | 🔒 | `{ text }` | `guid` |
| DELETE | `/api/comments/{id}` | 🔒 author | — | 200 |
| GET | `/api/videos/{videoId}/comments?page=&pageSize=` | — | — | `{ items: [{id,authorId,text,createdAt}], totalCount, page, pageSize }` |

### `/api/recommendations`
| Method | Auth | Response |
|---|---|---|
| GET `?videoId=&limit=` | optional 🔒 | `VideoDto[]` |

Logic: `videoId` given → tag-overlap related videos. No `videoId` but authenticated → personalized from like history. Neither → popular fallback. Always backfilled with popular videos if short of `limit`. Private videos never appear here regardless of path. No `page` param — this endpoint is not paginated.

---

## Auth flow summary
1. `POST /register` → `POST /confirm-email` → `POST /login` → store `accessToken`+`refreshToken`.
2. Attach `Authorization: Bearer <accessToken>` to every 🔒 request (and to `GET /videos/{id}` too, even though it's optional — see private-video note above).
3. On `401`, call `POST /refresh` with the stored `refreshToken`, retry once.
4. `POST /logout` on sign-out.

## Video publish flow
1. `POST /api/videos/upload` → get `videoId` (`status: "Uploading"`, no sources yet).
2. `POST /api/media/upload` (`mediaType=RawVideo`, that `videoId`) → get back `{ mediaAssetId, bucket, key }`, async processing starts.
3. Poll `GET /api/media/{mediaAssetId}/status` for status-based UX. Once `Completed`, poll `GET /api/videos/{id}` — `status` will have flipped to `"Draft"` and `sources` will be populated. If `status` comes back `"Failed"`, show `failureReason` and offer delete+retry.
4. `PATCH /api/videos/{id}/publish` with desired visibility.
5. Later, to change visibility on this already-published video: `PATCH /api/videos/{id}/visibility` (not `/publish` again).

## Channel avatar/banner flow
1. `POST /api/media/upload` (`mediaType=Avatar` or `Banner`, `channelId` = current user's id) → returns immediately `{ mediaAssetId, status: "Completed", bucket, key }`.
2. `PATCH /api/channels/{id}` with `avatarBucket`/`avatarKey` (or `bannerBucket`/`bannerKey`) set to the values from step 1. Omit the other pair entirely if you're not changing it — it won't be cleared.
