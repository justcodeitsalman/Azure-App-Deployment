using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Controllers;
using UserManagement.Web.Models.Users;

namespace UserManagement.Data.Tests;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUserService> _userService = null!;
    private UsersController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _userService = new Mock<IUserService>();
        _controller = new UsersController(_userService.Object);
    }

    [Test]
    public void List_ShowAll_UsesGetAllWithNullAndReturnsAll()
    {
        // Arrange
        var all = StubUsers(true, false, true).ToList();

        _userService
            .Setup(s => s.GetAllAsync(null))
            .ReturnsAsync(all);

        // Act
        var result = _controller.List(null).Result;

        // Assert
        var view = result as ViewResult;
        Assert.That(view, Is.Not.Null);

        var model = view!.Model as UserListViewModel;
        Assert.That(model, Is.Not.Null);

        Assert.That(model!.Items.Count, Is.EqualTo(all.Count));
    }


    [Test]
    public void List_WithIsActiveTrue_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var active = StubUsers(true).ToList();

        _userService.Setup(s => s.GetAllAsync(true)).ReturnsAsync(active);


        // Act
        var result = _controller.List(true).Result;

        // Assert
        var view = result as ViewResult;
        Assert.That(view, Is.Not.Null);

        var model = view!.Model as UserListViewModel;
        Assert.That(model, Is.Not.Null);

        Assert.That(model!.Items.Count, Is.EqualTo(1));
        Assert.That(model!.Items.TrueForAll(i => i.IsActive), Is.True);
    }

    [Test]
    public void List_WithIsActiveFalse_ShouldReturnOnlyInactiveUsers()
    {
        // Arrange
        var inactive = StubUsers(false).ToList();

        _userService.Setup(s => s.GetAllAsync(false)).ReturnsAsync(inactive);


        // Act
        var result = _controller.List(false).Result;

        // Assert
        var view = result as ViewResult;
        Assert.That(view, Is.Not.Null);

        var model = view!.Model as UserListViewModel;
        Assert.That(model, Is.Not.Null);

        Assert.That(model!.Items.Count, Is.EqualTo(1));
        Assert.That(model!.Items.TrueForAll(i => !i.IsActive), Is.True);
    }

    // ---------- helpers ----------
    private static User[] StubUsers(params bool[] actives)
    {
        var list = new List<User>();
        for (int i = 0; i < actives.Length; i++)
        {
            list.Add(new User
            {
                Id = i + 1,
                Forename = "User",
                Surname = "Test",
                Email = $"user{i + 1}@example.com",
                IsActive = actives[i]
            });
        }
        return list.ToArray();
    }
}


