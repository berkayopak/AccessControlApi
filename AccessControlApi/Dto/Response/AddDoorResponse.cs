using AccessControlApi.Domain;

namespace AccessControlApi.Dto.Response;

public class AddDoorResponse
{
    public string DoorId { get; }
    public string DoorName { get; }

    public AddDoorResponse(Door door)
    {
        DoorId = door.Id;
        DoorName = door.Name;
    }
}