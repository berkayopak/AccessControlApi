using AccessControlApi.Domain;
using AccessControlApi.Dto.Request;
using AccessControlApi.Dto.Response;

namespace AccessControlApi.Facade;

public interface IDoorFacade
{
    public Task<AddDoorResponse> Add(Door door);
    public Task<AssignRoleToDoorResponse> AssignRole(DoorRole doorRole);
    public Task<OpenDoorResponse> Open(string doorId, string userId, IReadOnlyCollection<string> userRoles);
    public Task<BasePaginationResponse<GetDoorEventHistoryResponse>> GetDoorEventHistory(GetDoorEventHistoryRequest request);
}