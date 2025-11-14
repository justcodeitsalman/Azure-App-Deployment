using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public interface IDataContext
{
    // Users table (DbSet) in the database.
    DbSet<User> Users { get; }
  
    // Persists all pending changes to the database.
    Task<int> SaveChangesAsync();
}
