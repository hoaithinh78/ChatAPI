using ChatR.DTOs.Dashboard;
using ChatR.DTOs.Message;
using ChatR.DTOs.Server;
using ChatR.DTOs.User;

namespace ChatR.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int userId);
        Task<List<MyServerDto>> GetMyServersAsync(int userId);
        Task<List<RecentServerDto>> GetRecentServersAsync(int userId);
        Task<List<DirectMessageDto>> GetDirectMessagesAsync(int userId);
        Task<CurrentUserDto> GetCurrentUserAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
    }
}
