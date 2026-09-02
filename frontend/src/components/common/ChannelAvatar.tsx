import { Link } from 'react-router-dom';
import { useChannel } from '@/hooks/useChannel';

interface ChannelAvatarProps {
  channelId: string;
  size?: number;
  linkToChannel?: boolean;
}

export function ChannelAvatar({ channelId, size = 36, linkToChannel = true }: ChannelAvatarProps) {
  const { data: channel } = useChannel(channelId);
  const style = { height: size, width: size };

  const img = channel?.avatarUrl ? (
    <img src={channel.avatarUrl} alt="" style={style} className="rounded-full object-cover" />
  ) : (
    <div style={style} className="rounded-full bg-surface-raised" aria-hidden />
  );

  if (!linkToChannel) return img;

  return (
    <Link to={`/channel/${channelId}`} className="block shrink-0">
      {img}
    </Link>
  );
}