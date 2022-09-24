using AccessControlApi.Common;
using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class AssignRoleToUserRequest
{
    public string? RoleName { get; set; }
    public string? UserId { get; set; }
    public ValidationResult Validate() => new AssignRoleToUserRequestValidator().Validate(this);
}

public class AssignRoleToUserRequestValidator : AbstractValidator<AssignRoleToUserRequest>
{
    public AssignRoleToUserRequestValidator()
    {
        RuleFor(r => r.RoleName).NotEmpty();
        RuleFor(r => r.RoleName).Must(roleName => RoleName.RoleNames.Contains(roleName))
            .WithMessage("The role you entered is not defined in the system.");
        RuleFor(r => r.UserId).NotEmpty();
    }
}