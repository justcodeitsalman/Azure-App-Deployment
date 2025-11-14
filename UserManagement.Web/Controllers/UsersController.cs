using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;


[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    /// <summary>
    /// List users. Optional ?isActive=true/false filter.
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> List([FromQuery] bool? isActive)
    {
        var users = await _userService.GetAllAsync(isActive);

        var items = users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive,
            DateOfBirth = u.DateOfBirth
        }).ToList();

        var model = new UserListViewModel
        {
            Items = items
        };

        return View(model);
    }

    /// <summary>
    /// Shows details for a single user.
    /// GET /users/view/5
    /// </summary>
    [HttpGet("view/{id:long}")]
    public async Task<IActionResult> ViewUser(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);  // Strongly-typed view taking User as model
    }

    /// <summary>
    /// Shows the "create new user" form.
    /// GET /users/create
    /// </summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        // Use the domain model directly because it has DataAnnotations.
        return View(new User());
    }

    /// <summary>
    /// Handles submission of the "create new user" form.
    /// POST /users/create
    /// </summary>
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User model)
    {
        if (!ModelState.IsValid)
        {
            // Redisplay form with validation errors.
            return View(model);
        }

        await _userService.CreateAsync(model);
        return RedirectToAction(nameof(List));
    }

    /// <summary>
    /// Shows the "edit user" form.
    /// GET /users/edit/5
    /// </summary>
    [HttpGet("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    /// <summary>
    /// Handles submission of the "edit user" form.
    /// POST /users/edit/5
    /// </summary>
    [HttpPost("edit/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, User model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var success = await _userService.UpdateAsync(model);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(List));
    }

    /// <summary>
    /// Shows a confirmation screen before deleting a user.
    /// GET /users/delete/5
    /// </summary>
    [HttpGet("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    /// <summary>
    /// Handles the delete confirmation.
    /// POST /users/delete/5
    /// </summary>
    [HttpPost("delete/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var success = await _userService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(List));
    }
}
