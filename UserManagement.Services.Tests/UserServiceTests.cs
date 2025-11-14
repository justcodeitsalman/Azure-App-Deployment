using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private DataContext _context = null!;
        private UserService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new DataContext(options);
            _service = new UserService(_context);
        }

        // ------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------
        [Test]
        public async Task GetAllAsync_WhenUsersExist_ShouldReturnAll()
        {
            // Arrange
            _context.Users.AddRange(new[]
            {
                new User { Forename = "A", Surname = "Test", Email = "a@test.com", IsActive = true },
                new User { Forename = "B", Surname = "Test", Email = "b@test.com", IsActive = false }
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_WhenFilterActive_ShouldReturnOnlyActive()
        {
            // Arrange
            _context.Users.AddRange(new[]
            {
                new User { Forename = "A", Surname = "Test", Email = "a@test.com", IsActive = true },
                new User { Forename = "B", Surname = "Test", Email = "b@test.com", IsActive = false }
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(true);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.All(u => u.IsActive), Is.True);
        }

        [Test]
        public async Task GetAllAsync_WhenFilterInactive_ShouldReturnOnlyInactive()
        {
            // Arrange
            _context.Users.AddRange(new[]
            {
                new User { Forename = "A", Surname = "Test", Email = "a@test.com", IsActive = true },
                new User { Forename = "B", Surname = "Test", Email = "b@test.com", IsActive = false }
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync(false);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.All(u => !u.IsActive), Is.True);
        }

        // ------------------------------------------------------
        // GET BY ID
        // ------------------------------------------------------
        [Test]
        public async Task GetByIdAsync_WhenExists_ShouldReturnUser()
        {
            var user = new User { Forename = "A", Surname = "Test", Email = "a@test.com", IsActive = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(user.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Email, Is.EqualTo(user.Email));
        }

        [Test]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            var result = await _service.GetByIdAsync(999);
            Assert.That(result, Is.Null);
        }

        // ------------------------------------------------------
        // CREATE
        // ------------------------------------------------------
        [Test]
        public async Task CreateAsync_ShouldAddUserAndReturnIt()
        {
            var user = new User { Forename = "John", Surname = "Doe", Email = "jd@test.com" };

            var result = await _service.CreateAsync(user);

            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.IsActive, Is.True); // service forces new users active
            Assert.That(_context.Users.Count(), Is.EqualTo(1));
        }

        // ------------------------------------------------------
        // UPDATE
        // ------------------------------------------------------
        [Test]
        public async Task UpdateAsync_WhenExists_ShouldUpdateValues()
        {
            var user = new User { Forename = "Old", Surname = "User", Email = "old@test.com", IsActive = true };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.Forename = "New";
            user.Email = "new@test.com";

            var result = await _service.UpdateAsync(user);

            Assert.That(result, Is.True);

            var updated = await _context.Users.FindAsync(user.Id);
            Assert.That(updated!.Forename, Is.EqualTo("New"));
            Assert.That(updated.Email, Is.EqualTo("new@test.com"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnFalse()
        {
            var user = new User { Id = 999, Forename = "Nope" };
            var result = await _service.UpdateAsync(user);

            Assert.That(result, Is.False);
        }

        // ------------------------------------------------------
        // DELETE
        // ------------------------------------------------------
        [Test]
        public async Task DeleteAsync_WhenExists_ShouldDeleteUser()
        {
            var user = new User { Forename = "A", Surname = "B", Email = "ab@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(user.Id);

            Assert.That(result, Is.True);
            Assert.That(_context.Users.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnFalse()
        {
            var result = await _service.DeleteAsync(999);
            Assert.That(result, Is.False);
        }
    }
}
