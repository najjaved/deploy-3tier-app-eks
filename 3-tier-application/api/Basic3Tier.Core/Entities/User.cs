using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Basic3Tier.Core;

[Table("user")]
public class User : CommonEntity
{
    [Column("name")]
    public string Name { get; set; }

    [Column("age")]
    public int Age { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("address")]
    [AllowNull]
    public string Address { get; set; }
}
