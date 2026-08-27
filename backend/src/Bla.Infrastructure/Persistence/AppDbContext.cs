using Bla.Application.Common.Interfaces;
using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bla.Infrastructure.Persistence;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ICurrentUser currentUser) : DbContext(options), IAppDbContext
{
    private Guid CurrentUserId => currentUser.Id;

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.ToTable("users"); builder.HasKey(user => user.Id);
            builder.Property(user => user.Email).HasMaxLength(320); builder.Property(user => user.DisplayName).HasMaxLength(200);
        });
        modelBuilder.Entity<TaskItem>(builder =>
        {
            builder.ToTable("tasks"); builder.HasKey(task => task.Id);
            builder.Property(task => task.Title).HasMaxLength(120).IsRequired(); builder.Property(task => task.Description).HasMaxLength(2000);
            builder.Property(task => task.Status).HasConversion<string>().HasMaxLength(20).IsRequired(); builder.Property(task => task.DueDate).HasColumnType("date");
            builder.HasIndex(task => new { task.OwnerId, task.Status, task.DueDate });
            builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(task => task.OwnerId).OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(task => task.OwnerId == CurrentUserId);
        });
    }
}
