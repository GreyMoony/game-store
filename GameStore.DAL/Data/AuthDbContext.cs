using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Data;
public class AuthDbContext(DbContextOptions<AuthDbContext> options) :
    IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<UserNotificationMethod> UserNotificationMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserNotificationMethod>()
            .HasIndex(x => new { x.UserId, x.Method })
            .IsUnique();

        builder.Entity<UserNotificationMethod>()
            .Property(x => x.Method)
            .HasConversion<string>();

        builder.Entity<UserNotificationMethod>()
            .HasOne(n => n.User)
            .WithMany(u => u.NotificationMethods)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
