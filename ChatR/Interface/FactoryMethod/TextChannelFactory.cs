using ChatR.DTOs.Channels;
using ChatR.Enums;
using ChatR.Models;
namespace ChatR.Interface.FactoryMethod
{
    public class TextChannelFactory : IChannelFactory
    {
        public int SupportedType => (int)ChannelType.Text;

        public Channel Create(CreateChannelRequest request)
        {
            return new Channel
            {
                ChannelName = string.IsNullOrWhiteSpace(request.ChannelName)
                    ? "general"
                    : request.ChannelName.Trim(),
                Type = (int)ChannelType.Text,
                ServerId = request.ServerId,
                CreatedAt = DateTime.Now
            };
        }
    }
}
