using TaskManager.Domain;
using TaskManager.Application.Commands;
using TaskManager.Application;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace TaskManager.Tests;

public class BusinessRulesTests
{
    private class InMemoryDb : DbContext, Application.IApplicationDbContext
    {
        public InMemoryDb(DbContextOptions options) : base(options) { }
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskRelation> TaskRelations => Set<TaskRelation>();
    }

    private static InMemoryDb CreateDb()
    {
        var opts = new DbContextOptionsBuilder<InMemoryDb>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new InMemoryDb(opts);
    }

    [Fact]
    public async Task Cannot_Relate_Task_To_Itself()
    {
        using var db = CreateDb();
        var task = new TaskItem{ Title="A", Author="u"};
        db.Tasks.Add(task); await db.SaveChangesAsync();

        var handler = new AddRelationHandler(db);
        await Assert.ThrowsAsync<System.InvalidOperationException>(() => handler.Handle(new AddRelationCommand(task.Id, task.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_Task_With_Subtasks_Fails()
    {
        using var db = CreateDb();
        var parent = new TaskItem{ Title="P", Author="u"};
        var child = new TaskItem{ Title="C", Author="u", ParentTask = parent };
        db.Tasks.AddRange(parent, child); await db.SaveChangesAsync();

        var handler = new DeleteTaskHandler(db);
        await Assert.ThrowsAsync<System.InvalidOperationException>(() => handler.Handle(new DeleteTaskCommand(parent.Id), CancellationToken.None));
    }
}
