using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;

using TaskManager.Application.Commands;
using TaskManager.Domain;
using TaskManager.Infrastructure.Persistence;

using Xunit;

namespace TaskManager.Tests;

public class BusinessRulesTests
{
    private static AppDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task Cannot_Relate_Task_To_Itself()
    {
        using var db = CreateDb();

        var task = new TaskItem{ Title="A", Author="u"};
        db.Tasks.Add(task); await db.SaveChangesAsync();

        var handler = new AddRelationHandler(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new AddRelationCommand(task.Id, task.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_Task_With_Subtasks_Fails()
    {
        using var db = CreateDb();

        var parent = new TaskItem{ Title="P", Author="u"};
        var child = new TaskItem{ Title="C", Author="u", ParentTask = parent };
        db.Tasks.AddRange(parent, child); await db.SaveChangesAsync();

        var handler = new DeleteTaskHandler(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new DeleteTaskCommand(parent.Id), CancellationToken.None));
    }
}
