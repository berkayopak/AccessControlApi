using AccessControlApi.Dto.Request;
using AccessControlApi.Config;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AccessControlApi.Common;
using AccessControlApi.Domain;
using AccessControlApi.Helper;
using Microsoft.AspNetCore.Identity;
using AccessControlApi.Dto.Response;

namespace AccessControlApi.Facade;

public class UserFacade : IUserFacade
{
    private readonly ApplicationConfig _config;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UserFacade(
        IOptions<ApplicationConfig> config,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _config = config.Value;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new CustomApplicationException("User not found!", StatusCodes.Status401Unauthorized);

        //P.S: In case of need, functions to add data to UserLogins and UserTokens tables can be called from this section.
        //I haven't added it for now to keep it as simple as possible.

        var userRoles = (await _userManager.GetRolesAsync(user))?.ToList();

        var claimList = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (userRoles is not null)
            claimList.AddRange(from roleName in userRoles
                where !string.IsNullOrWhiteSpace(roleName)
                select new Claim(ClaimTypes.Role, roleName));

        const int expirationMinute = 300;
        var jwtToken = JwtHelper.GetToken(_config.Auth.Secret, expirationMinute, claimList);

        //P.S: If you need UTC date, you can use DateTime.UtcNow
        return new AuthenticateResponse(jwtToken, DateTime.Now.AddMinutes(300));
    }

    public async Task<CreateUserResponse> CreateBasicUser(CreateUserRequest request, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is not null)
            throw new CustomApplicationException(
                "This email address is currently in use. Please enter another email address.",
                StatusCodes.Status409Conflict);
        User newUser = new()
        {
            UserName = request.Username,
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        var createUserResult = await _userManager.CreateAsync(newUser, request.Password);

        if (!createUserResult.Succeeded)
        {
            var createUserErrors = string.Join(" ", createUserResult.Errors.Select(e => e.Description));
            throw new CustomApplicationException(
                $"An error was encountered, while creating the user! Please check your registration details and try again. Error: {createUserErrors}",
                StatusCodes.Status422UnprocessableEntity);
        }
        
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new Role(roleName));
            if (!createRoleResult.Succeeded)
                throw new Exception($"An error was encountered, while creating the {roleName} role!");
        }

        var createUserRoleResult = await _userManager.AddToRoleAsync(newUser, roleName);
        if (!createUserRoleResult.Succeeded)
            throw new Exception(
                $"An error was encountered, while assigning the {roleName} role to user({newUser.Email})!");

        return new CreateUserResponse(newUser.Email!, true);
    }

    public async Task<AssignRoleToUserResponse> AssignRole(AssignRoleToUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            throw new CustomApplicationException(
                "User not found.",
                StatusCodes.Status400BadRequest);

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new Role(request.RoleName!));
            if (!createRoleResult.Succeeded)
                throw new Exception($"An error was encountered, while creating the {request.RoleName} role!");
        }

        if (await _userManager.IsInRoleAsync(user, request.RoleName))
            throw new CustomApplicationException(
                "The operation could not be performed because the user already owns the role entered.",
                StatusCodes.Status409Conflict);

        var createUserRoleResult = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!createUserRoleResult.Succeeded)
            throw new Exception(
                $"An error was encountered, while assigning the {request.RoleName} role to user({user.Email})!");

        return new AssignRoleToUserResponse(user.Id, request.RoleName!);
    }
}