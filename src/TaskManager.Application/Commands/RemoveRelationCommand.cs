using MediatR;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Application.Commands;

public record RemoveRelationCommand(Guid TaskId, Guid RelatedTaskId) : IRequest;

public class RemoveRelationHandler(IApplicationDbContext db) : IRequestHandler<RemoveRelationCommand>
{
    public async Task Handle(RemoveRelationCommand r, CancellationToken ct)
    {
        var rels = await db.TaskRelations.Where(x => 
            (x.TaskId == r.TaskId && x.RelatedTaskId == r.RelatedTaskId) ||
            (x.TaskId == r.RelatedTaskId && x.RelatedTaskId == r.TaskId)).ToListAsync(ct);

        if (rels.Count == 0) return;

        db.TaskRelations.RemoveRange(rels);
        await db.SaveChangesAsync(ct);
    }
}
