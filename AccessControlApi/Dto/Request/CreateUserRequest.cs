using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class CreateUserRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    public ValidationResult Validate() => new RegisterRequestValidator().Validate(this);
}

public class RegisterRequestValidator : AbstractValidator<CreateUserRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Username).NotEmpty().MinimumLength(2).MaximumLength(32);
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.Password).NotEmpty().MinimumLength(8)
            .Matches(new Regex(@"((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z])(?=.*[.!@#&()–[\]\/\+\*{}:;',?~$^=<>]).*$"));
    }
}