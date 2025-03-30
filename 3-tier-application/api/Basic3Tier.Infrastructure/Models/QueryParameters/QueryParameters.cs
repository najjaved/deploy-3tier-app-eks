using Basic3Tier.Core;

namespace Basic3Tier.Infrastructure.Models;

public class QueryParameters : Serializable
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public string OrderBy { get; set; }
}
