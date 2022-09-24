using System.Security.Claims;
using AccessControlApi.Common;
using AccessControlApi.Dto.Request;
using AccessControlApi.Dto.Response;
using AccessControlApi.Facade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
//Api version information is taken from here, even if we don't give it,
//it would get 1.0 by default, but I wanted to show its usage.
[ApiVersion("1.0")]
public class DoorController : ControllerBase
{
    private readonly IDoorFacade _doorFacade;
    
    public DoorController(IDoorFacade doorFacade) =>
        _doorFacade = doorFacade;

    [Authorize(Roles = RoleName.SuperAdmin)]
    [HttpPost("Add")]
    public async Task<ActionResult<OperationResult<AddDoorResponse>>> Add(AddDoorRequest request)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        var addDoorResponse = await _doorFacade.Add(request.ToDoor());
        return Ok(new OperationResult<AddDoorResponse>(addDoorResponse, true, null, validationResult, StatusCodes.Status200OK));
    }

    [Authorize(Roles = RoleName.SuperAdmin)]
    [HttpPost("AssignRole")]
    public async Task<ActionResult<OperationResult<AssignRoleToDoorResponse>>> AssignRole(AssignRoleToDoorRequest request)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        var assignRoleToDoorResponse = await _doorFacade.AssignRole(request.ToDoorRole());
        return Ok(new OperationResult<AssignRoleToDoorResponse>(assignRoleToDoorResponse, true, null, validationResult,
            StatusCodes.Status200OK));
    }
    
    [HttpGet("Open")]
    public async Task<ActionResult<OperationResult<OpenDoorResponse>>> Open([FromQuery]OpenDoorRequest request)
    {
        var userRoleList = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        //DoorId already validated with FluentValidation, so we know its not null.
        var openDoorResponse = await _doorFacade.Open(request.DoorId!, userId, userRoleList);

        if (!openDoorResponse.Succeeded)
            return Unauthorized(new OperationResult<OpenDoorResponse>(openDoorResponse, false,
                "You do not have the necessary role to open the door.", validationResult,
                StatusCodes.Status403Forbidden));

        return Ok(new OperationResult<OpenDoorResponse>(openDoorResponse, true, null, validationResult, StatusCodes.Status200OK));
    }

    [Authorize(Roles = $"{RoleName.Admin},{RoleName.SuperAdmin}")]
    [HttpGet("EventHistory")]
    public async Task<ActionResult<OperationResult<BasePaginationResponse<GetDoorEventHistoryResponse>>>> GetEventHistory([FromQuery] GetDoorEventHistoryRequest request)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        return Ok(new OperationResult<BasePaginationResponse<GetDoorEventHistoryResponse>>(
            await _doorFacade.GetDoorEventHistory(request), true, null, validationResult, StatusCodes.Status200OK));
    }
}