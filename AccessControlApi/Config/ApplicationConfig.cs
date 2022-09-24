namespace AccessControlApi.Config;

public class ApplicationConfig
{
    public const string Application = "Application";
    public AuthConfig Auth { get; set; } = null!;

    public class AuthConfig
    {
        public string Secret { get; set; } = null!;
    }
}