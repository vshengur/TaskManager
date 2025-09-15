using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Commands;
using TaskManager.Application.DTOs;
using TaskManager.Application.Queries;
using TaskManager.Domain;

using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // all endpoints require JWT
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    public TasksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> Get([FromQuery] string? assignee, [FromQuery] TaskStatus? status, [FromQuery] TaskPriority? priority, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTasksQuery(assignee, status, priority), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetTaskByIdQuery(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskCommand cmd, CancellationToken ct)
    {
        var dto = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, [FromBody] UpdateTaskCommand cmd, CancellationToken ct)
    {
        if (id != cmd.Id) return BadRequest("Mismatched id");
        var dto = await _mediator.Send(cmd, ct);
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTaskCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/relations/{relatedId:guid}")]
    public async Task<IActionResult> AddRelation(Guid id, Guid relatedId, CancellationToken ct)
    {
        await _mediator.Send(new AddRelationCommand(id, relatedId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/relations/{relatedId:guid}")]
    public async Task<IActionResult> RemoveRelation(Guid id, Guid relatedId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveRelationCommand(id, relatedId), ct);
        return NoContent();
    }
}
