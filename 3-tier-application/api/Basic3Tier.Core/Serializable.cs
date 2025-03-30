using Newtonsoft.Json;

namespace Basic3Tier.Core;

public abstract class Serializable
{
    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
