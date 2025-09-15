using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Application.Common;
using TaskManager.Domain;
using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Application.Commands;

public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string? Description,
    string Author,
    string? Assignee,
    TaskStatus Status,
    TaskPriority Priority,
    Guid? ParentTaskId
) : IRequest<TaskDto>;

public class UpdateTaskHandler(IApplicationDbContext db) : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    public async Task<TaskDto> Handle(UpdateTaskCommand r, CancellationToken ct)
    {
        var e = await db.Tasks.FirstOrDefaultAsync(x => x.Id == r.Id, ct) ?? throw new KeyNotFoundException("Task not found");
        if (r.ParentTaskId == e.Id) throw new InvalidOperationException("Task cannot be its own parent.");

        e.Title = r.Title;
        e.Description = r.Description;
        e.Author = r.Author;
        e.Assignee = r.Assignee;
        e.Status = r.Status;
        e.Priority = r.Priority;
        e.ParentTaskId = r.ParentTaskId;

        await db.SaveChangesAsync(ct);
        return e.ToDto();
    }
}
