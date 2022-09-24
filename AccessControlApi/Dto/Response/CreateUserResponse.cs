namespace AccessControlApi.Dto.Response;

public class CreateUserResponse
{
    public string UserEmail { get; }
    public bool Succeeded { get; }

    public CreateUserResponse(string userEmail, bool succeeded)
    {
        UserEmail = userEmail;
        Succeeded = succeeded;
    }
}