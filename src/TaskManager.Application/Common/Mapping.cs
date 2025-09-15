using TaskManager.Application.DTOs;
using TaskManager.Domain;

namespace TaskManager.Application.Common;

public static class Mapping
{
    public static TaskDto ToDto(this TaskItem e) =>
        new TaskDto(e.Id, e.Title, e.Description, e.Author, e.Assignee, e.Status, e.Priority, e.ParentTaskId);
}
