using AccessControlApi.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class AddDoorRequest
{
    //I made it nullable because I want this variable to be passed to FluentValidation under all conditions.
    public string? Name { get; set; }

    public ValidationResult Validate() => new AddDoorRequestValidator().Validate(this);

    public Door ToDoor() => new() { Name = Name!, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
}

public class AddDoorRequestValidator : AbstractValidator<AddDoorRequest>
{
    public AddDoorRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(128);
    }
}