namespace Basic3Tier.Core.Extensions;

public static class Extensions
{
    public static DateTime SetKindUtc(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) return dateTime;
        return dateTime.ToUniversalTime();
    }

}
