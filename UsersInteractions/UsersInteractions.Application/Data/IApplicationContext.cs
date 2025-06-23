using Microsoft.EntityFrameworkCore;
using UsersInteractions.Domain.Models;

namespace UsersInteractions.Application.Data;

public interface IApplicationContext
{
    DbSet<ChatMessage> ChatMessages { get; set; }
    DbSet<Comment> Comments { get; set; }

    IApplicationContext CreateDbContext();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}