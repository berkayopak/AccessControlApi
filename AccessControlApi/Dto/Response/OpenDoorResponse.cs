namespace AccessControlApi.Dto.Response;

public class OpenDoorResponse
{
    public string DoorId { get; }
    public bool Succeeded { get; }

    public OpenDoorResponse(string doorId, bool succeeded)
    {
        DoorId = doorId;
        Succeeded = succeeded;
    }
}