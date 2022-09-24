using AccessControlApi.Domain;
using AccessControlApi.Dto.Source;

namespace AccessControlApi.Repository;

public interface IDoorRepository
{
    public Task<Door> Add(Door door);
    public Task<DoorRole> AssignRole(DoorRole doorRole);
    public Task<bool> IsDoorRoleExists(DoorRole doorRole);
    public Task<bool> HasDoorAnyMatchingRole(string doorId, IReadOnlyCollection<string> roleList);
    public Task<Door?> Find(string doorId);
    public Task<DoorEventHistory> AddEventToHistory(DoorEventHistory doorEventHistory);
    public Task<int> GetDoorEventHistoryCount(GetDoorEventHistoryFilter filter);
    public Task<IReadOnlyCollection<DoorEventHistory>> GetDoorEventHistory(GetDoorEventHistoryFilter filter);
}