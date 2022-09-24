using AccessControlApi.Domain;

namespace AccessControlApi.Dto.Source;

public class GetDoorEventHistoryFilter : BasePaginationFilter
{
    public IReadOnlyCollection<string>? DoorIds { get; }
    public IReadOnlyCollection<string>? UserIds { get; }
    public DoorEventType? DoorEventType { get; }
    public bool? Succeeded { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public GetDoorEventHistoryFilter(
        int pageSize,
        int pageNumber,
        IReadOnlyCollection<string>? doorIds,
        IReadOnlyCollection<string>? userIds,
        DoorEventType? doorEventType,
        bool? succeeded,
        DateTime? startDate,
        DateTime? endDate) : base(pageSize, pageNumber)
    {
        DoorIds = doorIds;
        UserIds = userIds;
        DoorEventType = doorEventType;
        Succeeded = succeeded;
        StartDate = startDate;
        EndDate = endDate;
    }
}