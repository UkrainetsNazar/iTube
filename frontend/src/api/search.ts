import { apiClient } from './client';
import type { SearchResponse, VideoSearchHit } from '@/types';

interface RawSearchHit {
  videoId: string;
  title: string;
  description: string;
  tags: string[];
  thumbnailUrl: string | null;
  viewsCount: number;
  score: number;
  authorId: string;
  publishedAt: string;
}

interface RawSearchResponse {
  hits: RawSearchHit[];
  totalCount: number;
}

function mapHit(raw: RawSearchHit): VideoSearchHit {
  return {
    id: raw.videoId,
    title: raw.title,
    description: raw.description,
    thumbnailUrl: raw.thumbnailUrl,
    viewsCount: raw.viewsCount,
    authorId: raw.authorId,
    tags: raw.tags,
    score: raw.score,
    createdAt: raw.publishedAt,
  };
}

export const searchApi = {
  search: (q: string, tags: string[], page: number, pageSize: number) =>
    apiClient
      .get<RawSearchResponse>('/videos/search', {
        params: {
          q: q.trim() ? q.trim() : undefined,
          tags: tags.length ? tags.join(',') : undefined,
          page,
          pageSize,
        },
      })
      .then(
        (r): SearchResponse => ({
          hits: r.data.hits.map(mapHit),
          totalCount: r.data.totalCount,
        })
      ),
};