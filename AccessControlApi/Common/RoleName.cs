namespace AccessControlApi.Common;

public static class RoleName
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Basic = "Basic";

    public static IReadOnlyCollection<string> RoleNames => new[] { Basic, Admin, SuperAdmin };
}