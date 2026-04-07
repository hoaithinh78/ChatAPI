using System.Security.Claims;

namespace ChatR.Server.Helpers
{
    public static class CurrentUserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(claim))
                throw new Exception("Không tìm thấy user id trong token");

            return int.Parse(claim);
        }
    }
}
