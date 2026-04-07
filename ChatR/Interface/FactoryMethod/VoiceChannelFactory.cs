using ChatR.DTOs.Channels;
using ChatR.Enums;
using ChatR.Models;
namespace ChatR.Interface.FactoryMethod
{
    public class VoiceChannelFactory : IChannelFactory
    {
        public int SupportedType => (int)ChannelType.Voice;

        public Channel Create(CreateChannelRequest request)
        {
            return new Channel
            {
                ChannelName = string.IsNullOrWhiteSpace(request.ChannelName)
                    ? "Voice Room"
                    : request.ChannelName.Trim(),
                Type = (int)ChannelType.Voice,
                ServerId = request.ServerId,
                CreatedAt = DateTime.Now
            };
        }
    }
}
