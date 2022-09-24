using AccessControlApi.Domain;

namespace AccessControlApi.Dto.Response;

public class AssignRoleToDoorResponse
{
    public string DoorId { get; }
    public string RoleId { get; }

    public AssignRoleToDoorResponse(DoorRole doorRole)
    {
        DoorId = doorRole.DoorId;
        RoleId = doorRole.RoleId;
    }
}