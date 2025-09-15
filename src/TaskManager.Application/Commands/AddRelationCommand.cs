using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Application.Commands;

public record AddRelationCommand(Guid TaskId, Guid RelatedTaskId) : IRequest;

public class AddRelationHandler(IApplicationDbContext db) : IRequestHandler<AddRelationCommand>
{
    public async Task Handle(AddRelationCommand r, CancellationToken ct)
    {
        if (r.TaskId == r.RelatedTaskId) throw new InvalidOperationException("Task cannot be related to itself.");

        var exists = await db.TaskRelations.AnyAsync(x => 
            (x.TaskId == r.TaskId && x.RelatedTaskId == r.RelatedTaskId) ||
            (x.TaskId == r.RelatedTaskId && x.RelatedTaskId == r.TaskId), ct);

        if (exists) return;

        db.TaskRelations.Add(new TaskRelation { TaskId = r.TaskId, RelatedTaskId = r.RelatedTaskId });
        db.TaskRelations.Add(new TaskRelation { TaskId = r.RelatedTaskId, RelatedTaskId = r.TaskId });
        await db.SaveChangesAsync(ct);
    }
}
