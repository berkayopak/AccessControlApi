namespace AccessControlApi.Dto.Response;

public class AuthenticateResponse
{
    public string? Token { get; }
    public DateTime ExpirationDate { get; }

    public AuthenticateResponse(string? token, DateTime expirationDate)
    {
        Token = token;
        ExpirationDate=expirationDate;
    }
}