using ChatR.DTOs.Channels;
using ChatR.Models;
namespace ChatR.Interface.FactoryMethod
{
    public interface IChannelFactory
    {
        int SupportedType { get; }
        Channel Create(CreateChannelRequest request);
    }

}
