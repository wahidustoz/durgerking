using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace DurgerKing.Entity.Data;
interface IAppDbContext
{
    public DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}