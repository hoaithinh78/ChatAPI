using ChatR.Data;
using ChatR.DTOs.Channels;
using ChatR.Interface.FactoryMethod;
using ChatR.Models;
using Microsoft.EntityFrameworkCore;
namespace ChatR.Services.Server
{
    public class ChannelService
    {
        private readonly AppDbContext _context;
        private readonly ChannelFactoryProvider _channelFactoryProvider;

        public ChannelService(AppDbContext context, ChannelFactoryProvider channelFactoryProvider)
        {
            _context = context;
            _channelFactoryProvider = channelFactoryProvider;
        }

        public async Task<Channel> CreateChannelAsync(CreateChannelRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ChannelName))
            {
                request.ChannelName = "new-channel";
            }

            var serverExists = await _context.Servers
                .AnyAsync(s => s.ServerId == request.ServerId);

            if (!serverExists)
            {
                throw new Exception("Server không tồn tại.");
            }

            var duplicated = await _context.Channels.AnyAsync(c =>
                c.ServerId == request.ServerId &&
                c.ChannelName!.ToLower() == request.ChannelName.Trim().ToLower());

            if (duplicated)
            {
                throw new Exception("Tên channel đã tồn tại trong server.");
            }

            var factory = _channelFactoryProvider.GetFactory(request.Type);
            var channel = factory.Create(request);

            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();

            return channel;
        }

        public async Task<List<Channel>> GetChannelsByServerIdAsync(int serverId)
        {
            return await _context.Channels
                .Where(c => c.ServerId == serverId)
                .OrderBy(c => c.Type)
                .ThenBy(c => c.ChannelName)
                .ToListAsync();
        }
    }
}
