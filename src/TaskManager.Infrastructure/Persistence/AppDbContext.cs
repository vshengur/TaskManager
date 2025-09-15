using Microsoft.EntityFrameworkCore;
using TaskManager.Application;
using TaskManager.Domain;

namespace TaskManager.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskRelation> TaskRelations => Set<TaskRelation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Title).HasMaxLength(200).IsRequired();
            b.Property(t => t.Description).HasMaxLength(4000);
            b.Property(t => t.Author).HasMaxLength(100).IsRequired();
            b.Property(t => t.Assignee).HasMaxLength(100);

            b.HasOne(t => t.ParentTask)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskRelation>(b =>
        {
            b.HasKey(r => new { r.TaskId, r.RelatedTaskId });
            b.HasOne(r => r.Task).WithMany(t => t.Related).HasForeignKey(r => r.TaskId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(r => r.RelatedTask).WithMany(t => t.RelatedOf).HasForeignKey(r => r.RelatedTaskId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
