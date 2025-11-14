using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Filters by active state when param is supplied.
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns>All users</returns>
    Task<List<User>> GetAllAsync(bool? isActive = null);

    /// <summary>
    /// Returns a single user by id, or null if not found.
    /// </summary>
    Task<User?> GetByIdAsync(long id);

    /// <summary>
    /// Creates a new user and saves it to the database.
    /// </summary>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Updates an existing user. Returns false if the user does not exist.
    /// </summary>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// Deletes a user by id. Returns false if the user does not exist.
    /// </summary>
    Task<bool> DeleteAsync(long id);

}
