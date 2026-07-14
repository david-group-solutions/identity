namespace DavidGroup.Core.Identity.Samples.WebApi.Data;

public static class Permissions
{
    public static class Books
    {
        public const string Read = $"{nameof(Books)}.{nameof(Read)}";
    }
}
