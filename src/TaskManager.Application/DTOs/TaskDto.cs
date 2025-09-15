using TaskManager.Domain;

using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    string Author,
    string? Assignee,
    TaskStatus Status,
    TaskPriority Priority,
    Guid? ParentTaskId);
