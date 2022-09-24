using AccessControlApi.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class AssignRoleToDoorRequest
{
    //I made it nullable because I want this variable to be passed to FluentValidation under all conditions.
    public string? RoleId { get; set; }
    public string? DoorId { get; set; }

    public ValidationResult Validate() => new AssignRoleToDoorRequestValidator().Validate(this);

    public DoorRole ToDoorRole() => new() { RoleId = RoleId!, DoorId = DoorId!};
}

public class AssignRoleToDoorRequestValidator : AbstractValidator<AssignRoleToDoorRequest>
{
    public AssignRoleToDoorRequestValidator()
    {
        RuleFor(r => r.RoleId).NotEmpty();
        RuleFor(r => r.DoorId).NotEmpty();
    }
}