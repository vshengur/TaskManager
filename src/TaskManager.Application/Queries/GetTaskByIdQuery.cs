using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Application.Common;

namespace TaskManager.Application.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto?>;

public class GetTaskByIdHandler(IApplicationDbContext db) : IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken ct)
    {
        var entity = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        return entity?.ToDto();
    }
}
