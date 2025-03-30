
namespace Basic3Tier.Core.Tests;

public class UserRepositoryTest 
{
    [Theory]
    [InlineData("Test", 22, "Random Address", "testuser@xunit.com")]
    public async void UserAddSuccess(string name, int age, string address, string email)
    {
        using TestHelper testHelper = new();
        var userRepository = testHelper.GetInMemoryUserRepository();

        var user = new User
        {
            Name = name,
            Age = age,
            Address = address,
            Email = email,
            CreatedOn = DateTime.UtcNow
        };

        var status = userRepository.Add(user);
        await userRepository.SaveChangesAsync();

        Assert.True(status);
    }

    [Theory]
    [InlineData("Test", 22, "Random Address", "testuser@xunit.com")]
    public async void UserGetSingleSuccess(string name, int age, string address, string email)
    {
        using TestHelper testHelper = new();
        var userRepository = testHelper.GetInMemoryUserRepository();

        var user = new User
        {
            Name = name,
            Age = age,
            Address = address,
            Email = email,
            CreatedOn = DateTime.UtcNow
        };

        var status = userRepository.Add(user);
        await userRepository.SaveChangesAsync();

        var savedUser = await userRepository.GetSingleAsync(u => u.Name == name);
        Assert.NotNull(savedUser);
    }
}
