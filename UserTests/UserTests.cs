using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Services;
using Xunit;

namespace ProiectPSSC2025.Tests
{
    public class UserTests
    {
        // 1. DOMAIN TESTS

        [Fact] 
        public void SettingEmail_ValidEmail()
        {
            // Arrange
            var user = new UserDTO();

            // Act
            user.SetEmail("test@example.com");

            // Assert
            Assert.Equal("test@example.com", user.Email);
        }

        [Fact]
        public void SettingEmail_Invalid()
        {
            // Arrange
            var user = new UserDTO();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => user.SetEmail("invalidEmail"));
        }

        // 2. UNIT TESTS: UserService

        [Fact]
        public async Task GetAllUsersAsync_UserDTOs()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
                new User { Id = "2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" }
            };

            var mockRepository = new Mock<IUserRepository>();
            mockRepository
                .Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            var userService = new UserService(mockRepository.Object);

            // Act
            var result = await userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Id == "1" && r.Email == "user1@example.com");
            Assert.Contains(result, r => r.Id == "2" && r.Email == "user2@example.com");
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDTO()
        {
            // Arrange
            var mockRepository = new Mock<IUserRepository>();
            mockRepository
                .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    Id = "123",
                    Email = "test@example.com",
                    FirstName = "test",
                    LastName = "User"
                });

            var userService = new UserService(mockRepository.Object);

            // Act
            var result = await userService.GetUserByIdAsync("anyId");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123", result.Id);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("test", result.FirstName);
            Assert.Equal("User", result.LastName);
        }

        [Fact]
        public async Task AddUserAsync_Repository()
        {
            // Arrange
            var mockRepository = new Mock<IUserRepository>();
            var userService = new UserService(mockRepository.Object);
            var newUser = new User { Email = "mock@example.com" };

            // Act
            await userService.AddUserAsync(newUser);

            // Assert
            mockRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        // 3. DUMMY TEST

        [Fact]
        public async Task AddUserAsync_WithDummyUser()
        {
            // Arrange
            var mockRepository = new Mock<IUserRepository>();
            var dummyUser = new User();

            var userService = new UserService(mockRepository.Object);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => userService.AddUserAsync(dummyUser));
            Assert.Null(exception);
        }

        // 4. STUB TEST

        [Fact]
        public async Task GetUserByIdAsync()
        {
            // Arrange
            var stubRepository = new Mock<IUserRepository>();
            stubRepository
                .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    Id = "28",
                    Email = "test@example.com"
                });

            var userService = new UserService(stubRepository.Object);

            // Act
            var result = await userService.GetUserByIdAsync("anyId");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("28", result.Id);
            Assert.Equal("test@example.com", result.Email);
        }

        // 5. MOCK TEST

        [Fact]
        public async Task GetAllUsersAsync()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetAllUsersAsync())
                    .ReturnsAsync(Enumerable.Empty<User>());

            var userService = new UserService(mockRepo.Object);

            // Act
            var users = await userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(users);
            mockRepo.Verify(r => r.GetAllUsersAsync(), Times.Once);
        }

        // 6. FAKE TEST

        private class FakeUserRepository : IUserRepository
        {
            private readonly List<User> _users = new();

            public Task<IEnumerable<User>> GetAllUsersAsync()
            {
                return Task.FromResult(_users.AsEnumerable());
            }

            public Task<User?> GetUserByIdAsync(string id)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            }

            public Task AddUserAsync(User user)
            {
                _users.Add(user);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task UserService_WithFakeRepository_CanAddAndViewUsers()
        {
            // Arrange
            var fakeRepo = new FakeUserRepository();
            var userService = new UserService(fakeRepo);

            var newUser = new User { Id = "ABC", Email = "fake@example.com" };

            // Act
            await userService.AddUserAsync(newUser);
            var allUsers = await userService.GetAllUsersAsync();
            var retrievedUser = await userService.GetUserByIdAsync("ABC");

            // Assert
            Assert.Single(allUsers);  // only one user added
            Assert.NotNull(retrievedUser);
            Assert.Equal("fake@example.com", retrievedUser!.Email);
        }

        // 7. FRAMEWORK MOCKING EXAMPLE

        [Fact]
        public async Task FrameworkMockingTest()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(r => r.GetAllUsersAsync())
                .ReturnsAsync(new List<User>
                {
                    new User { Id = "1", Email = "test1@example.com" },
                    new User { Id = "2", Email = "test2@example.com" }
                });

            var service = new UserService(mockRepo.Object);

            // Act
            var users = await service.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, users.Count());
            mockRepo.Verify(r => r.GetAllUsersAsync(), Times.Once);
        }
    }
}
