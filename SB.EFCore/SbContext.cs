using Microsoft.EntityFrameworkCore;
using SB.EFCore.Entities;

namespace SB.EFCore
{
    public class SbContext : DbContext
    {
        public SbContext(DbContextOptions<SbContext> options) : base(options)
        {
        }
        
        public DbSet<TicketEntity> Tickets { get; set; }
        
        public DbSet<UserInfoEntity> UserInfoEntities { get; set; }
        
        public DbSet<AnswerEntity> Answers { get; set; }
        
        public DbSet<AttachmentEntity> Attachments { get; set; }

        public DbSet<SupporterEntity> Supporters { get; set; }
        
        public DbSet<LiveChatEntity> LiveChats { get; set; }
        
        public DbSet<MessageEntity> Messages { get; set; }
    }
}