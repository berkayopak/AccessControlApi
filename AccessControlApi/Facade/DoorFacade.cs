using AccessControlApi.Common;
using AccessControlApi.Domain;
using AccessControlApi.Dto.Request;
using AccessControlApi.Dto.Response;
using AccessControlApi.Dto.Source;
using AccessControlApi.Repository;
using Microsoft.AspNetCore.Identity;

namespace AccessControlApi.Facade;

public class DoorFacade : IDoorFacade
{
    private readonly IDoorRepository _doorRepository;
    private readonly RoleManager<Role> _roleManager;

    public DoorFacade(IDoorRepository doorRepository, RoleManager<Role> roleManager)
    {
        _doorRepository = doorRepository;
        _roleManager = roleManager;
    }

    public async Task<AddDoorResponse> Add(Door door) => new(await _doorRepository.Add(door));

    public async Task<AssignRoleToDoorResponse> AssignRole(DoorRole doorRole)
    {
        var door = await _doorRepository.Find(doorRole.DoorId);
        if (door is null)
            throw new CustomApplicationException("A door with the entered id value was not found.",
                StatusCodes.Status400BadRequest);

        var role = await _roleManager.FindByIdAsync(doorRole.RoleId);
        if (role is null)
            throw new CustomApplicationException("A role with the entered id value was not found.",
                StatusCodes.Status400BadRequest);

        var isExist = await _doorRepository.IsDoorRoleExists(doorRole);
        if (isExist)
            throw new CustomApplicationException("The role you are trying to assign to the gate already exists.",
                StatusCodes.Status409Conflict);

        return new AssignRoleToDoorResponse(await _doorRepository.AssignRole(doorRole));
    }

    public async Task<OpenDoorResponse> Open(string doorId, string userId, IReadOnlyCollection<string> userRoles)
    {
        var door = await _doorRepository.Find(doorId);

        if (door is null)
            throw new CustomApplicationException("A door with the entered id value was not found.",
                StatusCodes.Status400BadRequest);

        var hasDoorAnyMatchingRole = await _doorRepository.HasDoorAnyMatchingRole(doorId, userRoles);

        await _doorRepository.AddEventToHistory(new DoorEventHistory
        {
            AttemptedAt = DateTime.Now,
            DoorId = doorId,
            UserId = userId,
            EventType = DoorEventType.Open,
            Succeeded = hasDoorAnyMatchingRole,
            Information = hasDoorAnyMatchingRole
                ? "Door opened successfully"
                : "Could not open the door because the user did not have the necessary authorization to open the door."
        });

        return new OpenDoorResponse(doorId, hasDoorAnyMatchingRole);
    }

    public async Task<BasePaginationResponse<GetDoorEventHistoryResponse>> GetDoorEventHistory(
        GetDoorEventHistoryRequest request)
    {
        var getDoorEventHistoryFilter = new GetDoorEventHistoryFilter(
            request.PageSize,
            request.PageNumber,
            request.DoorIds,
            request.UserIds,
            request.DoorEventType,
            request.Succeeded,
            request.StartDate,
            request.EndDate);
        var totalItemCount = await _doorRepository.GetDoorEventHistoryCount(getDoorEventHistoryFilter);
        var totalPageCount = (int)Math.Ceiling((double)totalItemCount / request.PageSize);
        var doorEventHistoryList = await _doorRepository.GetDoorEventHistory(getDoorEventHistoryFilter);

        var getDoorEventHistoryResponse = new GetDoorEventHistoryResponse(doorEventHistoryList);

        return new BasePaginationResponse<GetDoorEventHistoryResponse>(getDoorEventHistoryResponse, totalItemCount,
            totalPageCount, request.PageNumber, request.PageSize);
    }
}