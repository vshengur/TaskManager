using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Application.Commands;

public record DeleteTaskCommand(Guid Id) : IRequest;

public class DeleteTaskHandler(IApplicationDbContext db) : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand r, CancellationToken ct)
    {
        var e = await db.Tasks.Include(t => t.Subtasks)
            .FirstOrDefaultAsync(x => x.Id == r.Id, ct) 
            ?? throw new KeyNotFoundException("Task not found");

        if (e.Subtasks.Count != 0)
            throw new InvalidOperationException("Cannot delete task with subtasks. Reassign or delete subtasks first.");

        db.Tasks.Remove(e);
        await db.SaveChangesAsync(ct);
    }
}
