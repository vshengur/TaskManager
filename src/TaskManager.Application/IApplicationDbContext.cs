using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Application;

public interface IApplicationDbContext
{
    DbSet<TaskItem> Tasks { get; }
    DbSet<TaskRelation> TaskRelations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
