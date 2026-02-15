using LittleHeroesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<AvailableReward> AvailableRewards => Set<AvailableReward>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<MyAchievement> MyAchievements => Set<MyAchievement>();
    public DbSet<SuperSwat> SuperSwats => Set<SuperSwat>();
    public DbSet<LevelMadAchievement> LevelMadAchievements => Set<LevelMadAchievement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasIndex(e => e.Auth0UserId).IsUnique();
            entity.Property(e => e.Auth0UserId).HasMaxLength(128);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.HasOne(e => e.Group).WithMany(g => g.Parents).HasForeignKey(e => e.GroupId);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.Property(e => e.ChildName).HasMaxLength(100);
            entity.Property(e => e.Reward).HasMaxLength(256);
            entity.Property(e => e.Avatar).HasMaxLength(512);
            entity.HasOne(e => e.Group).WithMany(g => g.Children).HasForeignKey(e => e.GroupId);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasIndex(e => e.ChildId).IsUnique();
            entity.HasOne(e => e.Child).WithOne(c => c.Pet).HasForeignKey<Pet>(e => e.ChildId);
        });

        modelBuilder.Entity<AvailableReward>(entity =>
        {
            entity.Property(e => e.Reward).HasMaxLength(256);
            entity.HasOne(e => e.Child).WithMany(c => c.AvailableRewards).HasForeignKey(e => e.ChildId);
        });

        modelBuilder.Entity<MyAchievement>(entity =>
        {
            entity.HasOne(e => e.Child).WithMany(c => c.MyAchievements).HasForeignKey(e => e.ChildId);
            entity.HasOne(e => e.Achievement).WithMany().HasForeignKey(e => e.AchievementsId);
        });

        modelBuilder.Entity<SuperSwat>(entity =>
        {
            entity.HasIndex(e => e.ChildId).IsUnique();
            entity.HasOne(e => e.Child).WithOne(c => c.SuperSwat).HasForeignKey<SuperSwat>(e => e.ChildId);
        });

        modelBuilder.Entity<LevelMadAchievement>(entity =>
        {
            entity.HasIndex(e => e.ChildId).IsUnique();
            entity.HasOne(e => e.Child).WithOne(c => c.LevelMadAchievement).HasForeignKey<LevelMadAchievement>(e => e.ChildId);
        });

        // Seed achievements
        modelBuilder.Entity<Achievement>().HasData(
            new Achievement { Id = 1, Title = "Super Swat", Description = "Earn points on 5 consecutive days" },
            new Achievement { Id = 2, Title = "Mega Points", Description = "Earn 50 points in one day" },
            new Achievement { Id = 3, Title = "Level Mad", Description = "Level up twice in one day" },
            new Achievement { Id = 4, Title = "Make a Monster", Description = "Customize your monster" }
        );
    }
}
