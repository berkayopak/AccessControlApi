using FluentValidation;

namespace AccessControlApi.Dto.Request;

public abstract class BasePaginationRequest
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}

public class BasePaginationRequestValidator : AbstractValidator<BasePaginationRequest>
{
    public BasePaginationRequestValidator()
    {
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
    }
}