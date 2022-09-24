using AccessControlApi.Common;
using AccessControlApi.Config;
using AccessControlApi.Controllers;
using AccessControlApi.Domain;
using AccessControlApi.Dto.Request;
using AccessControlApi.Dto.Response;
using AccessControlApi.Facade;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace AccessControlApi.Test;

public class UserTest
{
    private readonly IOptions<ApplicationConfig> _config;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<Role>> _roleManagerMock;

    public UserTest()
    {
        var applicationConfig = new ApplicationConfig
        {
            Auth = new ApplicationConfig.AuthConfig
            {
                Secret = "8sdNgYjWXTzVCIRk71tSPkEdK6vIZfKW"
            }
        };
        _config = Options.Create(applicationConfig);
        _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _roleManagerMock = new Mock<RoleManager<Role>>(Mock.Of<IRoleStore<Role>>(), null, null, null, null);
    }

    [Fact]
    public async void Create()
    {
        var password = "12345678Be.";
        User newUser = new()
        {
            UserName = "berkayopak",
            Email = "berkayopak@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))!
            .ReturnsAsync((User?)null);

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var userFacade = new UserFacade(_config, _userManagerMock.Object, _roleManagerMock.Object);
        var userController = new UserController(userFacade);

        var createUserRequest = new CreateUserRequest
        {
            Username = newUser.UserName,
            Email = newUser.Email,
            Password = password
        };

        var expectedCreateUserResponse = new CreateUserResponse(newUser.Email, true);

        var actionResult = await userController.CreateBasicUser(createUserRequest);
        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var operationResult = Assert.IsAssignableFrom<OperationResult<CreateUserResponse>>(response.Value);

        Assert.NotNull(response);
        Assert.True(operationResult.Success);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedCreateUserResponse),
            JsonConvert.SerializeObject(operationResult.Data));
    }

    [Fact]
    public async void AssignRole()
    {
        var roleName = "Admin";
        User user = new()
        {
            Id = "f8389291-a097-4507-80dc-663790c1d971",
            UserName = "berkayopak",
            Email = "berkayopak@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var userFacade = new UserFacade(_config, _userManagerMock.Object, _roleManagerMock.Object);
        var userController = new UserController(userFacade);

        var assignRoleToUserRequest = new AssignRoleToUserRequest
        {
            UserId = user.Id,
            RoleName = roleName
        };

        var expectedAssignRoleToUserResponse = new AssignRoleToUserResponse(user.Id, roleName);

        var actionResult = await userController.AssignRole(assignRoleToUserRequest);
        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var operationResult = Assert.IsAssignableFrom<OperationResult<AssignRoleToUserResponse>>(response.Value);

        Assert.NotNull(response);
        Assert.True(operationResult.Success);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedAssignRoleToUserResponse),
            JsonConvert.SerializeObject(operationResult.Data));
    }
}