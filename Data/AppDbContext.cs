using ChatR.Server.Models;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace ChatR.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Friend> Friends => Set<Friend>();
        public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
        public DbSet<Servers> Servers => Set<Servers>();

        public DbSet<ServerMember> ServerMembers => Set<ServerMember>();
        public DbSet<Channel> Channels => Set<Channel>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<MessageRead> MessageReads => Set<MessageRead>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Invite> Invites => Set<Invite>();

        // Cấu hình mối quan hệ giữa các bảng
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");
                entity.HasKey(x => x.UserId);
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
                entity.Property(x => x.Email).HasColumnName("EMAIL");
                entity.Property(x => x.Username).HasColumnName("USERNAME");
                entity.Property(x => x.Password).HasColumnName("PASSWORD");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.Status).HasColumnName("STATUS");
                entity.Property(x => x.NumberPhone).HasColumnName("NUMBER_PHONE");
                entity.Property(x => x.DisplayName).HasColumnName("DISPLAY_NAME");
                entity.Property(x => x.AvatarUrl).HasColumnName("AVATAR_URL");
                entity.Property(x => x.Bio).HasColumnName("BIO");
            });

            modelBuilder.Entity<Friend>(entity =>
            {
                entity.ToTable("FRIENDS");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("ID");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
                entity.Property(x => x.FriendId).HasColumnName("FRIEND_ID");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
            });
            modelBuilder.Entity<Friend>()
                .HasOne(f => f.UserInfo) // Người gửi
                .WithMany() // Người gửi có thể có nhiều bạn bè
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Cấm xóa người dùng khi có bạn bè

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.FriendInfo) // Người nhận
                .WithMany() // Người nhận có thể có nhiều bạn bè
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.ToTable("FRIEND_REQUESTS");
                entity.HasKey(x => x.RequestId);
                entity.Property(x => x.RequestId).HasColumnName("REQUEST_ID");
                entity.Property(x => x.SenderId).HasColumnName("SENDER_ID");
                entity.Property(x => x.ReceiverId).HasColumnName("RECEIVER_ID");
                entity.Property(x => x.Status).HasColumnName("STATUS");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
            });

            modelBuilder.Entity<Servers>(entity =>
            {
                entity.ToTable("SERVERS");
                entity.HasKey(x => x.ServerId);
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
                entity.Property(x => x.ServerName).HasColumnName("SERVER_NAME");
                entity.Property(x => x.OwnerId).HasColumnName("OWNER_ID");
                entity.Property(x => x.Description).HasColumnName("DESCRIPTION");
                entity.Property(x => x.TotalMembers).HasColumnName("TOTAL_MEMBERS");
                entity.Property(x => x.OnlineMembers).HasColumnName("ONLINE_MEMBERS");
                entity.Property(x => x.Score).HasColumnName("SCORE");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.IconUrl).HasColumnName("ICON_URL");
            });

            modelBuilder.Entity<ServerMember>(entity =>
            {
                entity.ToTable("SERVER_MEMBERS");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("ID");
                entity.Property(x => x.JoinedAt).HasColumnName("JOINED_AT");
                entity.Property(x => x.IsOnline).HasColumnName("IS_ONLINE");
                entity.Property(x => x.Nickname).HasColumnName("NICKNAME");
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
                entity.HasIndex(x => new { x.ServerId, x.UserId }).IsUnique();
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.ToTable("CHANNELS");
                entity.HasKey(x => x.ChannelId);
                entity.Property(x => x.ChannelId).HasColumnName("CHANNEL_ID");
                entity.Property(x => x.ChannelName).HasColumnName("CHANNEL_NAME");
                entity.Property(x => x.Type).HasColumnName("TYPE");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLES");
                entity.HasKey(x => x.RoleId);
                entity.Property(x => x.RoleId).HasColumnName("ROLE_ID");
                entity.Property(x => x.RoleName).HasColumnName("ROLE_NAME");
                entity.Property(x => x.Position).HasColumnName("POSITION");
                entity.Property(x => x.Color).HasColumnName("COLOR");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("PERMISSIONS");
                entity.HasKey(x => x.PermissionId);
                entity.Property(x => x.PermissionId).HasColumnName("PERMISSION_ID");
                entity.Property(x => x.Code).HasColumnName("CODE");
                entity.Property(x => x.Description).HasColumnName("DESCRIPTION");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.Status).HasColumnName("STATUS");
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("ROLE_PERMISSIONS");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("ID");
                entity.Property(x => x.RoleId).HasColumnName("ROLE_ID");
                entity.Property(x => x.PermissionId).HasColumnName("PERMISSION_ID");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("USER_ROLE");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("ID");
                entity.Property(x => x.RoleId).HasColumnName("ROLE_ID");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
                entity.HasIndex(x => new { x.UserId, x.RoleId, x.ServerId }).IsUnique();
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ToTable("CONVERSATIONS");
                entity.HasKey(x => x.ConversationId);
                entity.Property(x => x.ConversationId).HasColumnName("CONVERSATION_ID");
                entity.Property(x => x.Type).HasColumnName("TYPE");
                entity.Property(x => x.Name).HasColumnName("NAME");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
            });

            modelBuilder.Entity<ConversationMember>(entity =>
            {
                entity.ToTable("CONVERSATION_MEMBERS");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("ID");
                entity.Property(x => x.ConversationId).HasColumnName("CONVERSATION_ID");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("MESSAGES");
                entity.HasKey(x => x.MessageId);
                entity.Property(x => x.MessageId).HasColumnName("MESSAGE_ID");
                entity.Property(x => x.SenderId).HasColumnName("SENDER_ID");
                entity.Property(x => x.Content).HasColumnName("CONTENT");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.EditedAt).HasColumnName("EDITED_AT");
                entity.Property(x => x.IsDeleted).HasColumnName("IS_DELETED");
                entity.Property(x => x.ChannelId).HasColumnName("CHANNEL_ID");
                entity.Property(x => x.ConversationId).HasColumnName("CONVERSATION_ID");
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("ATTACHMENTS");
                entity.HasKey(x => x.AttachId);
                entity.Property(x => x.AttachId).HasColumnName("ATTACH_ID");
                entity.Property(x => x.FileUrl).HasColumnName("FILE_URL");
                entity.Property(x => x.FileType).HasColumnName("FILE_TYPE");
                entity.Property(x => x.FileSize).HasColumnName("FILE_SIZE");
                entity.Property(x => x.MessageId).HasColumnName("MESSAGE_ID");
            });

            modelBuilder.Entity<MessageRead>(entity =>
            {
                entity.ToTable("MESSAGE_READS");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("ID");
                entity.Property(x => x.ReadAt).HasColumnName("READ_AT");
                entity.Property(x => x.MessageId).HasColumnName("MESSAGE_ID");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("NOTIFICATIONS");
                entity.HasKey(x => x.NotificationId);
                entity.Property(x => x.NotificationId).HasColumnName("NOTIFICATION_ID");
                entity.Property(x => x.Type).HasColumnName("TYPE");
                entity.Property(x => x.Content).HasColumnName("CONTENT");
                entity.Property(x => x.IsRead).HasColumnName("IS_READ");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AUDIT_LOG");
                entity.HasKey(x => x.LogId);
                entity.Property(x => x.LogId).HasColumnName("LOG_ID");
                entity.Property(x => x.Action).HasColumnName("ACTION");
                entity.Property(x => x.TargetType).HasColumnName("TARGET_TYPE");
                entity.Property(x => x.TargetId).HasColumnName("TARGET_ID");
                entity.Property(x => x.OldValue).HasColumnName("OLD_VALUE");
                entity.Property(x => x.NewValue).HasColumnName("NEW_VALUE");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.IpAddress).HasColumnName("IP_ADDRESS");
                entity.Property(x => x.DeviceInfo).HasColumnName("DEVICE_INFO");
                entity.Property(x => x.UserId).HasColumnName("USER_ID");
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
            });

            modelBuilder.Entity<Invite>(entity =>
            {
                entity.ToTable("INVITES");
                entity.HasKey(x => x.InviteId);
                entity.Property(x => x.InviteId).HasColumnName("INVITE_ID");
                entity.Property(x => x.Code).HasColumnName("CODE");
                entity.Property(x => x.CreatedAt).HasColumnName("CREATED_AT");
                entity.Property(x => x.ExpiresAt).HasColumnName("EXPIRES_AT");
                entity.Property(x => x.MaxUses).HasColumnName("MAX_USES");
                entity.Property(x => x.UsedCount).HasColumnName("USED_COUNT");
                entity.Property(x => x.CreatedBy).HasColumnName("CREATED_BY");
                entity.Property(x => x.ServerId).HasColumnName("SERVER_ID");
            });
        }
    }
}
