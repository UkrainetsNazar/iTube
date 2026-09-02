export type Role = 'User' | 'Moderator' | 'Admin';

export interface CurrentUser {
  id: string;
  email: string;
  role: Role;
}

export type VideoStatus = 'Draft' | 'Published' | 'Deleted';
export type Visibility = 'Public' | 'Unlisted' | 'Private';

export interface VideoSource {
  resolution: string;
  url: string;
  format: string;
}

export interface VideoDto {
  id: string;
  title: string;
  description: string;
  status: VideoStatus;
  visibility: Visibility;
  authorId: string;
  thumbnailUrl: string | null;
  viewsCount: number;
  likesCount: number;
  dislikesCount: number;
  tags: string[];
  sources: VideoSource[];
  createdAt: string;
  publishedAt: string | null;
}

export interface VideoSearchHit {
  id: string;
  title: string;
  description: string;
  thumbnailUrl: string | null;
  viewsCount: number;
  likesCount: number;
  tags: string[];
  score: number;
  createdAt: string;
}

export interface SearchResponse {
  hits: VideoSearchHit[];
  totalCount: number;
}

export interface CommentDto {
  id: string;
  authorId: string;
  text: string;
  createdAt: string;
}

export interface ChannelDto {
  id: string;
  name: string;
  description: string | null;
  avatarUrl?: string | null;
  bannerUrl?: string | null;
  subscribersCount: number;
  videoCount: number;
  status?: string;
}

export interface BanRecord {
  id: string;
  bannedByModeratorId: string;
  reason: string;
  bannedAt: string;
  expiresAt: string | null;
  isPermanent: boolean;
  isActive: boolean;
}

export interface UserDto {
  id: string;
  role: Role;
  status: string;
  createdAt: string;
  channel: {
    id: string;
    name: string;
    description: string | null;
    subscribersCount: number;
    videoCount: number;
    status: string;
  } | null;
  currentBan: BanRecord | null;
  banHistory: BanRecord[];
}

export interface AdminUserRow {
  userId: string;
  channelName: string;
  role: Role;
  status: string;
  isBanned: boolean;
  banReason: string | null;
  banExpiresAt: string | null;
  subscribersCount: number;
  createdAt: string;
}

export interface AdminUserPage {
  items: AdminUserRow[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Paged<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ApiError {
  code: string;
  message: string;
}

export type MediaType = 'RawVideo' | 'Avatar' | 'Banner';

export interface MediaUploadResponse {
  mediaAssetId: string;
  status: string;
  bucket: string;
  key: string;
}

export type MediaProcessingStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed';

export interface MediaStatusResponse {
  status: MediaProcessingStatus;
  failureReason?: string | null;
}

export interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

export type ReactionType = 'Like' | 'Dislike';
