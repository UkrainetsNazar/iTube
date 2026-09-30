# iTube

A YouTube-like video streaming platform: ASP.NET Core microservices backend, React + TypeScript frontend, event-driven communication via RabbitMQ, MinIO object storage, FFmpeg transcoding, and Elasticsearch search.

## Architecture

```
                        ┌──────────────┐
                        │  Frontend     │  (React + TS, nginx-served)
                        └──────┬────────┘
                               │
                        ┌──────▼────────────┐
                        │   ApiGateway       │
                        │ (YARP + JWT + CORS │
                        │  + Rate Limiting)  │
                        └─────────┬──────────┘
                                  │
      ┌───────────────┬──────────┼──────────┬───────────────┐
      ▼                ▼          ▼          ▼               ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│ AuthService │ │ UserService │ │MediaService │ │VideoService │
└──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬──────┘
       │               │               │               │
       └───────────────┴───────┬───────┴───────────────┘
                                ▼
                          RabbitMQ (events)
                                │
        ┌───────────┬──────────┼──────────┬───────────┐
        ▼           ▼          ▼          ▼           ▼
   PostgreSQL     Redis      MinIO   Elasticsearch  (per-service DBs)
   (×4, one per service)   (S3 API)  (search index)
```

Each backend service owns its own PostgreSQL database and talks to the others only through the Gateway (synchronous, from the frontend) or RabbitMQ integration events (asynchronous, service-to-service) — no service reaches into another's database directly. The frontend only ever talks to the Gateway.

## Services

| Service | Responsibility |
|---|---|
| **Frontend** | React SPA — home feed, watch page, search, channel pages, creator studio, admin panel |
| **ApiGateway** | Single entry point (YARP reverse proxy), JWT validation, CORS, rate limiting on auth/upload endpoints |
| **AuthService** | Registration, login, JWT issuing/refresh, email confirmation, password reset |
| **UserService** | User accounts, channels, subscriptions, roles, banning/moderation |
| **MediaService** | Raw file upload to MinIO, async FFmpeg transcoding (480p/720p/1080p), thumbnail generation |
| **VideoService** | Video metadata, comments, likes/dislikes, feeds, recommendations, watch history, full-text search |

Supporting infrastructure: **PostgreSQL** (isolated DB per service), **RabbitMQ** (event bus via MassTransit), **Redis** (view-count buffering), **MinIO** (S3-compatible object storage, public-read for served media), **Elasticsearch** (video search index).

## Tech stack

**Backend**: .NET 8 / ASP.NET Core, C# 12, EF Core + PostgreSQL, MediatR (CQRS) + FluentValidation, MassTransit + RabbitMQ, YARP, MinIO .NET SDK, FFmpeg (CLI), Elastic.Clients.Elasticsearch, StackExchange.Redis, Serilog. Domain-Driven Design throughout: aggregates, value objects, domain events, Result-based error handling.

**Frontend**: React + TypeScript + Vite, React Router, TanStack Query, Zustand (auth state), Axios, Tailwind CSS. No animation library — Tailwind keyframes only. See [`frontend/README.md`](./docs/Frontend-README.md) for frontend-specific architecture notes.

## Features

- 🎬 Video upload → async transcoding pipeline → multi-resolution playback, with a `Uploading → Draft → Published` (or `Failed`) status lifecycle
- 📺 Channels, subscriptions, subscription feed
- 👍 Likes/dislikes, comments
- 🔍 Full-text video search (Elasticsearch)
- 🧠 Content-based recommendations (tag overlap + personalization + popularity fallback)
- 📊 Watch history, liked-videos list, creator dashboard (draft/private/failed video management)
- 🔒 Private videos are fully hidden from everyone but the owner — excluded from feeds, search, and direct-link access alike
- 🛡️ Role-based moderation: ban/unban, role management, admin user search
- 🔐 JWT auth with refresh tokens, email confirmation, password reset
- 📈 YouTube-style view counting (30-second watch threshold, per-viewer cooldown) rather than count-on-click

## Getting started

### Prerequisites
- Docker + Docker Compose
- .NET 8 SDK (only for running/generating EF Core migrations outside Docker)
- Node.js 20+ (only for frontend local dev outside Docker)

### Setup

```bash
git clone <repo-url>
cd itube
cp backend/.env.example backend/.env   # review/edit values if needed
docker compose up -d --build
```

First build takes a while (Elasticsearch + FFmpeg install add real time). Once healthy:

| Service | URL |
|---|---|
| **Frontend** | http://localhost:5020 |
| API Gateway | http://localhost:5010 |
| AuthService (direct, debug only) | http://localhost:5001 |
| UserService (direct, debug only) | http://localhost:5002 |
| MediaService (direct, debug only) | http://localhost:5003 |
| VideoService (direct, debug only) | http://localhost:5004 |
| RabbitMQ management | http://localhost:15672 (guest/guest) |
| MinIO console | http://localhost:9001 (minioadmin/minioadmin) |
| Elasticsearch | http://localhost:9200 |

All browser traffic should go through the Frontend (`5020`) and, for API calls, the Gateway (`5010`). The direct service ports exist for local debugging and Swagger only.

### Verify it's running

```bash
docker compose ps          # all containers should show "healthy" or "Up"
docker compose logs -f api-gateway
```

Each backend service exposes Swagger UI in Development mode at `http://localhost:<port>/swagger`.

## Documentation

- [`docs/API-Reference.md`](./docs/API-Reference.md) — full endpoint reference (routes, auth, request/response shapes, known behaviors)
- [`frontend/README.md`](./docs/Frontend-README.md) — frontend architecture, patterns, and gaps against the backend contract

## Project structure

```
backend/
├── ApiGateway/
├── AuthService/       # API / Application / Domain / Infrastructure
├── UserService/       # same 4-layer structure
├── MediaService/       # same 4-layer structure
├── VideoService/       # same 4-layer structure
├── Shared.Domain/      # base entities, Result pattern, shared value objects, integration event contracts
├── Shared.Infrastructure/  # shared EF value converters
├── Shared.Application/     # shared MediatR validation pipeline behavior
├── Shared.Api/              # shared Result→HTTP mapping, global exception middleware
├── docker-compose.yml
├── .env.example
└── iTube.sln
frontend/
├── src/                # see frontend/README.md for the full breakdown
├── Dockerfile
└── nginx.conf
docs/
├── API-Reference.md
```

Each backend service follows Clean Architecture / DDD: **Domain** → **Application** (CQRS via MediatR) → **Infrastructure** (EF Core, external services, MassTransit consumers) → **API** (thin controllers).

## Running migrations

Each service's `Program.cs` applies pending migrations automatically on startup. To generate a new one after a model change:

```bash
dotnet ef migrations add <MigrationName> \
  --project backend/<Service>/<Service>.Infrastructure \
  --startup-project backend/<Service>/<Service>.API
```

## Known limitations

- **Cross-service consistency is eventual, not transactional.** Subscription state and per-channel published-video counts are replicated into other services via integration events, not queried live — there's a small window where services can briefly disagree.
- **`watchedSeconds` (view tracking) is client-reported, not independently verified.** Good enough for real usage; not fraud-proof.
- **Anonymous view/watch dedup doesn't exist.** Only signed-in viewers get the 4-hour per-video view cooldown; anonymous playback always counts.
- **FluentValidation pipeline is wired but sparsely populated.** The `ValidationBehavior<,>` MediatR pipeline behavior is registered in every service; most input validation still happens ad hoc inside command handlers via the `Result` pattern rather than dedicated `AbstractValidator<T>` classes.
- **Gateway JWT validation is a convenience layer, not the sole auth boundary.** Each service independently validates JWTs and enforces its own authorization policy.
- **Search index has no built-in staleness detection.** Visibility changes correctly update the index; other edits to a published video's title/description/tags do not currently re-trigger indexing.
