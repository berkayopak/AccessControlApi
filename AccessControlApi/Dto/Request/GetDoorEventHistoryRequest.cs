using AccessControlApi.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace AccessControlApi.Dto.Request;

public class GetDoorEventHistoryRequest : BasePaginationRequest
{
    public IReadOnlyCollection<string>? DoorIds { get; set; }
    public IReadOnlyCollection<string>? UserIds { get; set; }
    public DoorEventType? DoorEventType { get; set; }
    public bool? Succeeded { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ValidationResult Validate() => new GetDoorEventHistoryRequestValidator().Validate(this);
}

public class GetDoorEventHistoryRequestValidator : AbstractValidator<GetDoorEventHistoryRequest>
{
    public GetDoorEventHistoryRequestValidator()
    {
        Include(new BasePaginationRequestValidator());

        RuleForEach(r => r.DoorIds).NotEmpty()
            .When(r => r.DoorIds is not null);

        RuleForEach(r => r.UserIds).NotEmpty()
            .When(r => r.UserIds is not null);

        RuleFor(r => r.DoorEventType).IsInEnum()
            .When(r => r.DoorEventType is not null);

        RuleFor(r => r.EndDate).GreaterThanOrEqualTo(r => r.StartDate)
            .When(r => r.EndDate is not null && r.StartDate is not null);
    }
}