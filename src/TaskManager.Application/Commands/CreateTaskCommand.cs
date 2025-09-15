using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Common;
using TaskManager.Domain;

using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Application.Commands;

public record CreateTaskCommand(
    string Title,
    string? Description,
    string Author,
    string? Assignee,
    TaskStatus Status,
    TaskPriority Priority,
    Guid? ParentTaskId
) : IRequest<TaskDto>;

public class CreateTaskHandler(IApplicationDbContext db) : IRequestHandler<CreateTaskCommand, TaskDto>
{
    public async Task<TaskDto> Handle(CreateTaskCommand r, CancellationToken ct)
    {
        if (r.ParentTaskId is not null && r.ParentTaskId == Guid.Empty)
            throw new ArgumentException("ParentTaskId cannot be empty.");

        var entity = new TaskItem
        {
            Title = r.Title,
            Description = r.Description,
            Author = r.Author,
            Assignee = r.Assignee,
            Status = r.Status,
            Priority = r.Priority,
            ParentTaskId = r.ParentTaskId
        };

        db.Tasks.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }
}
