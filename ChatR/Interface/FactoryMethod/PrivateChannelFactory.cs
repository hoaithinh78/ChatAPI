using ChatR.DTOs.Channels;
using ChatR.Enums;
using ChatR.Models;
namespace ChatR.Interface.FactoryMethod
{
    public class PrivateChannelFactory : IChannelFactory
    {
        public int SupportedType => (int)ChannelType.Private;

        public Channel Create(CreateChannelRequest request)
        {
            return new Channel
            {
                ChannelName = string.IsNullOrWhiteSpace(request.ChannelName)
                    ? "private-channel"
                    : request.ChannelName.Trim(),
                Type = (int)ChannelType.Private,
                ServerId = request.ServerId,
                CreatedAt = DateTime.Now
            };
        }
    }
}
