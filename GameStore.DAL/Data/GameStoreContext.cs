using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Platform> Platforms { get; set; }

    public DbSet<GameGenre> GameGenres { get; set; }

    public DbSet<GamePlatform> GamePlatforms { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderGame> OrderGames { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<GameTranslation> GameTranslations { get; set; }

    public DbSet<LocalizedGame> LocalizedGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>()
            .HasIndex(g => g.Key)
            .IsUnique();

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PublisherId);

        modelBuilder.Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();

        modelBuilder.Entity<Genre>()
        .HasMany(g => g.SubGenres)
        .WithOne(g => g.ParentGenre)
        .HasForeignKey(g => g.ParentGenreId);

        modelBuilder.Entity<Platform>()
            .HasIndex(p => p.Type)
            .IsUnique();

        modelBuilder.Entity<GameGenre>()
            .HasKey(gg => new { gg.GameId, gg.GenreId });

        modelBuilder.Entity<GameGenre>()
            .HasOne(gg => gg.Game)
            .WithMany(g => g.GameGenres)
            .HasForeignKey(gg => gg.GameId);

        modelBuilder.Entity<GameGenre>()
            .HasOne(gg => gg.Genre)
            .WithMany(g => g.GameGenres)
            .HasForeignKey(gg => gg.GenreId);

        modelBuilder.Entity<GamePlatform>()
            .HasKey(gg => new { gg.GameId, gg.PlatformId });

        modelBuilder.Entity<GamePlatform>()
            .HasOne(gp => gp.Game)
            .WithMany(g => g.GamePlatforms)
            .HasForeignKey(gp => gp.GameId);

        modelBuilder.Entity<GamePlatform>()
            .HasOne(gp => gp.Platform)
            .WithMany(p => p.GamePlatforms)
            .HasForeignKey(gp => gp.PlatformId);

        modelBuilder.Entity<Publisher>()
            .HasIndex(p => p.CompanyName)
            .IsUnique();

        modelBuilder.Entity<OrderGame>()
            .HasIndex(og => new { og.OrderId, og.ProductId })
            .IsUnique();

        modelBuilder.Entity<OrderGame>()
            .HasOne(og => og.Order)
            .WithMany(o => o.OrderGames)
            .HasForeignKey(og => og.OrderId);

        modelBuilder.Entity<OrderGame>()
            .HasOne(og => og.Product)
            .WithMany(o => o.OrderGames)
            .HasForeignKey(og => og.ProductId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Game)
            .WithMany(g => g.Comments)
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(pc => pc.ChildComments)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GameTranslation>()
            .HasKey(gt => new { gt.GameId, gt.LanguageCode });

        modelBuilder.Entity<GameTranslation>()
            .HasOne(gt => gt.Game)
            .WithMany(g => g.Translations)
            .HasForeignKey(gt => gt.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LocalizedGame>().HasNoKey().ToView(null);

        modelBuilder.Entity<Game>().HasQueryFilter(g => !g.IsDeleted && !g.Publisher.IsDeleted);
        modelBuilder.Entity<Genre>().HasQueryFilter(g => !g.IsDeleted);
        modelBuilder.Entity<Platform>().HasQueryFilter(g => !g.IsDeleted);
        modelBuilder.Entity<Publisher>().HasQueryFilter(g => !g.IsDeleted);
        modelBuilder.Entity<GameGenre>().HasQueryFilter(gg => !gg.Game.IsDeleted && !gg.Genre.IsDeleted);
        modelBuilder.Entity<GamePlatform>().HasQueryFilter(gp => !gp.Game.IsDeleted && !gp.Platform.IsDeleted);
        modelBuilder.Entity<Comment>().HasQueryFilter(c => !c.Game.IsDeleted);
        modelBuilder.Entity<GameTranslation>().HasQueryFilter(gt => !gt.Game.IsDeleted);
    }
}
