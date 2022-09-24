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
public class UserController : ControllerBase
{
    private readonly IUserFacade _userFacade;
    public UserController(IUserFacade userFacade)
    {
        _userFacade = userFacade;
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public async Task<ActionResult<OperationResult<AuthenticateResponse>>> Authenticate(AuthenticateRequest request)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        var authenticateResponse = await _userFacade.Authenticate(request);

        return Ok(new OperationResult<AuthenticateResponse>(authenticateResponse, true, null, validationResult,
            StatusCodes.Status200OK));
    }

    [Authorize(Roles = RoleName.SuperAdmin)]
    [HttpPost("Create")]
    public async Task<ActionResult<OperationResult<CreateUserResponse>>> CreateBasicUser(CreateUserRequest request)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        var registerResponse = await _userFacade.CreateBasicUser(request, RoleName.Basic);
        return Ok(new OperationResult<CreateUserResponse>(registerResponse, true, null, validationResult, StatusCodes.Status200OK));
    }

    [Authorize(Roles = RoleName.SuperAdmin)]
    [HttpPost("AssignRole")]
    public async Task<ActionResult<OperationResult<AssignRoleToUserResponse>>> AssignRole(AssignRoleToUserRequest request)
    {
        var validationResult = request.Validate();
        if (!validationResult.IsValid)
            return BadRequest(new OperationResult<object?>(null, false, $"Invalid {nameof(request)}", validationResult,
                StatusCodes.Status400BadRequest));

        var assignRoleToUserResponse = await _userFacade.AssignRole(request);
        return Ok(new OperationResult<AssignRoleToUserResponse>(assignRoleToUserResponse, true, null, validationResult,
            StatusCodes.Status200OK));
    }
}