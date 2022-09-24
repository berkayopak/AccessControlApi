using AccessControlApi.Domain;
using Microsoft.OpenApi.Extensions;

namespace AccessControlApi.Dto.Response;

public class GetDoorEventHistoryResponse
{
    public IReadOnlyCollection<DoorEventHistoryResponse> DoorEventHistory { get; }

    public GetDoorEventHistoryResponse(IEnumerable<DoorEventHistory> doorEventHistory)
    {
        DoorEventHistory = doorEventHistory.Select(deh => new DoorEventHistoryResponse(deh)).ToList();
    }
}

public class DoorEventHistoryResponse
{
    public string DoorId { get; }
    public string DoorName { get; }
    public string UserId { get; }
    public string UserEmail { get; }
    public DoorEventType EventType { get; }
    public string EventTypeText { get; }
    public bool Succeeded { get; }
    public string SucceededText { get; }
    public string? Information { get; }
    public DateTime AttemptedAt { get; }
    public string AttemptedAtText { get; }

    public DoorEventHistoryResponse(DoorEventHistory doorEventHistory)
    {
        DoorId = doorEventHistory.DoorId;
        DoorName = doorEventHistory.Door!.Name;
        UserId = doorEventHistory.UserId;
        UserEmail = doorEventHistory.User!.Email;
        EventType = doorEventHistory.EventType;
        EventTypeText = doorEventHistory.EventType.GetDisplayName();
        Succeeded = doorEventHistory.Succeeded;
        SucceededText = doorEventHistory.Succeeded ? "Successful" : "Failed";
        Information = doorEventHistory.Information;
        AttemptedAt = doorEventHistory.AttemptedAt;
        AttemptedAtText = doorEventHistory.AttemptedAt.ToLongDateString();
    }
}