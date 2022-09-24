using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class AuthenticateRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }

    public ValidationResult Validate() => new AuthenticateRequestValidator().Validate(this);
}

public class AuthenticateRequestValidator : AbstractValidator<AuthenticateRequest>
{
    public AuthenticateRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty();
        RuleFor(r => r.Password).NotEmpty();
    }
}