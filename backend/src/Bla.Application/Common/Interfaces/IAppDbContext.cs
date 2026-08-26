using Bla.Domain.Identity;
using Bla.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bla.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<TaskItem> Tasks { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
