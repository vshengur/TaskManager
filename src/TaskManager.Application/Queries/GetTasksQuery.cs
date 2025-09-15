using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Application.Common;
using TaskManager.Domain;

using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Application.Queries;

public record GetTasksQuery(string? Assignee, TaskStatus? Status, TaskPriority? Priority) : IRequest<IReadOnlyList<TaskDto>>;

public class GetTasksHandler(IApplicationDbContext db) : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var q = db.Tasks.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Assignee))
            q = q.Where(t => t.Assignee == request.Assignee);
        if (request.Status is not null)
            q = q.Where(t => t.Status == request.Status);
        if (request.Priority is not null)
            q = q.Where(t => t.Priority == request.Priority);

        var items = await q
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.Status)
            .ToListAsync(ct);

        return [.. items.Select(i => i.ToDto())];
    }
}
