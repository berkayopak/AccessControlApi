using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class OpenDoorRequest
{
    public string? DoorId { get; set; }

    public ValidationResult Validate() => new OpenDoorRequestValidator().Validate(this);
}

public class OpenDoorRequestValidator : AbstractValidator<OpenDoorRequest>
{
    public OpenDoorRequestValidator()
    {
        RuleFor(r => r.DoorId).NotEmpty();
    }
}