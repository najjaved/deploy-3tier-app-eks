namespace Basic3Tier.Infrastructure;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string entity, string key, object value) :
        base($"Record in {entity} entity not found with {key}: {value}")
    {
        // nothing to do here
    }
}
