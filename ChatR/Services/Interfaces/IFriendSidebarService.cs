using ChatR.DTOs.Friends;

namespace ChatR.Services.Interfaces
{
    public interface IFriendSidebarService
    {

        Task<List<FriendListItemDto>> GetMyFriendsAsync(int userId);
        Task<List<FriendListItemDto>> GetOnlineFriendsAsync(int userId);
        Task<List<FriendSidebarItemDto>> GetFriendSidebarAsync(int userId);
    }
}
