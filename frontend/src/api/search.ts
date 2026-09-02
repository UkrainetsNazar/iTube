import { apiClient } from './client';
import type { SearchResponse } from '@/types';

export const searchApi = {
  search: (q: string, tags: string[], page: number, pageSize: number) =>
    apiClient
      .get<SearchResponse>('/videos/search', {
        params: {
          q,
          tags: tags.length ? tags.join(',') : undefined,
          page,
          pageSize,
        },
      })
      .then((r) => r.data),
};
