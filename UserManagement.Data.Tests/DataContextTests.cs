using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests
{
    public class DataContextTests
    {
        [Test]
        public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
        {
            // Arrange
            var context = CreateContext();

            var entity = new User
            {
                Forename = "Brand New",
                Surname = "User",
                Email = "brandnewuser@example.com",
                IsActive = true
            };

            context.Users.Add(entity);
            context.SaveChanges();

            // Act
            var results = context.Users.ToList();

            // Assert
            var found = results.Any(x => x.Email == entity.Email);
            Assert.That(found, Is.True, "Newly added entity should appear in GetAll");

            var retrieved = results.First(x => x.Email == entity.Email);

            Assert.Multiple(() =>
            {
                Assert.That(retrieved.Forename, Is.EqualTo(entity.Forename));
                Assert.That(retrieved.Surname, Is.EqualTo(entity.Surname));
                Assert.That(retrieved.Email, Is.EqualTo(entity.Email));
                Assert.That(retrieved.IsActive, Is.EqualTo(entity.IsActive));
            });
        }

        [Test]
        public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
        {
            // Arrange
            var context = CreateContext();

            var entity = new User
            {
                Forename = "Test",
                Surname = "User",
                Email = "testuser@example.com",
                IsActive = true
            };

            context.Users.Add(entity);
            context.SaveChanges();

            // Act – delete entity
            context.Users.Remove(entity);
            context.SaveChanges();

            var results = context.Users.ToList();

            // Assert
            var found = results.Any(x => x.Email == entity.Email);
            Assert.That(found, Is.False, "Deleted entity should not appear in GetAll");
        }

        private DataContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"Tests_{Guid.NewGuid()}")
                .Options;

            return new DataContext(options);
        }
    }
}
