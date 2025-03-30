using Basic3Tier.Core.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Basic3Tier.Core;

public abstract class CommonEntity : Serializable
{
    private DateTime? _createdOn;
    private DateTime? _updatedOn;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public virtual int Id { get; set; }

    [AllowNull]
    [Column("created_on")]
    public DateTime? CreatedOn { get => _createdOn; set => _createdOn = value?.SetKindUtc(); }

    [AllowNull]
    [Column("updated_on")]
    public DateTime? UpdatedOn { get => _updatedOn; set => _updatedOn = value?.SetKindUtc(); }

}
