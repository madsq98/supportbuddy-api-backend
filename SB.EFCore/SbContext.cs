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
    }
}