namespace Basic3Tier.Infrastructure.Models;

public class UserDtoRequest : CommonDtoRequest
{
    public string Name { get; set; }

    public int Age { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }
}
