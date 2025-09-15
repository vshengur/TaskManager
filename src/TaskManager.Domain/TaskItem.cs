using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(4000)]
    public string? Description { get; set; }

    [Required, MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Assignee { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.New;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public Guid? ParentTaskId { get; set; }
    public TaskItem? ParentTask { get; set; }
    public ICollection<TaskItem> Subtasks { get; set; } = [];

    // self-referencing many-to-many via join entity
    public ICollection<TaskRelation> Related { get; set; } = [];
    public ICollection<TaskRelation> RelatedOf { get; set; } = [];
}

public class TaskRelation
{
    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = default!;

    public Guid RelatedTaskId { get; set; }
    public TaskItem RelatedTask { get; set; } = default!;
}
