import { useQuery } from '@tanstack/react-query';
import { searchApi } from '@/api/search';

export function useSearch(q: string, tags: string[], page: number, pageSize = 24) {
  return useQuery({
    queryKey: ['search', q, tags, page, pageSize],
    queryFn: () => searchApi.search(q, tags, page, pageSize),
    enabled: q.trim().length > 0 || tags.length > 0,
  });
}