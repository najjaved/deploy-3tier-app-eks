using AutoMapper;
using Basic3Tier.Core;
using Basic3Tier.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Common;
using Moq;
using System.Linq.Expressions;

namespace Basic3Tier.Infrastructure.Tests
{
    public class UserServiceTest
    {
        private readonly Mock<ILogger<User>> moqLogger;
        private readonly IMapper mapper;
        private readonly Mock<IUserRepository> moqUserRepository;

        public UserServiceTest()
        {
            moqLogger = new Mock<ILogger<User>>();
            var myProfile = new UserMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);
            moqUserRepository = new Mock<IUserRepository>();
        }

        [Theory]
        [InlineData("Test", 22, "Random Address", "testuser@xunit.com")]
        public async void InsertEntitySuccess(string name, int age, string address, string email)
        {
            var user = new User
            {
                Id = 0,
                Name = name,
                Age = age,
                Address = address,
                Email = email
            };

            moqUserRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);
            moqUserRepository.Setup(x => x.GetWhereAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((new List<User> { user }).AsQueryable());
            moqUserRepository.Setup(x => x.Add(It.IsAny<User>())).Returns(true);

            var userService = new UserService(moqLogger.Object, mapper, moqUserRepository.Object);
            var userDtoRequest = new UserDtoRequest
            {
                Id = 0,
                Name = name,
                Age = age,
                Address = address,
                Email = email

            };
            var savedUser = await userService.InsertEntityAsync(userDtoRequest);
            Assert.NotNull(savedUser);
        }

        [Theory]
        [InlineData("Test", 22, "Random Address", "testuser@xunit.com")]
        public async void InsertEntityFailure(string name, int age, string address, string email)
        {
            var user = new User
            {
                Id = 0,
                Name = name,
                Age = age,
                Address = address,
                Email = email
            };

            moqUserRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);
            moqUserRepository.Setup(x => x.GetWhereAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((new List<User> { user }).AsQueryable());
            moqUserRepository.Setup(x => x.Add(It.IsAny<User>())).Returns(false);

            var userService = new UserService(moqLogger.Object, mapper, moqUserRepository.Object);
            var userDtoRequest = new UserDtoRequest
            {
                Id = 0,
                Name = name,
                Age = age,
                Address = address,
                Email = email

            };
            var savedUser = await userService.InsertEntityAsync(userDtoRequest);
            Assert.Null(savedUser);
        }

        [Theory]
        [InlineData(1, "Test", 22, "Random Address", "testuser@xunit.com")]
        public async void UpdateEntitySuccess(int id, string name, int age, string address, string email)
        {
            var user = new User
            {
                Id = id,
                Name = name,
                Age = age,
                Address = address,
                Email = email
            };

            moqUserRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);
            moqUserRepository.Setup(x => x.GetWhereAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((new List<User> { user }).AsQueryable());
            moqUserRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(true);

            var userService = new UserService(moqLogger.Object, mapper, moqUserRepository.Object);
            var userDtoRequest = new UserDtoRequest
            {
                Id = id,
                Name = name,
                Age = age,
                Address = address,
                Email = email

            };
            var savedUser = await userService.UpdateEntityAsync(id, userDtoRequest);
            Assert.NotNull(savedUser);
        }
    }
}