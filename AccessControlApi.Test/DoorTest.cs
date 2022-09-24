using System.Security.Claims;
using AccessControlApi.Domain;
using AccessControlApi.Repository;
using Microsoft.AspNetCore.Identity;
using Moq;
using AccessControlApi.Controllers;
using AccessControlApi.Dto.Request;
using AccessControlApi.Dto.Response;
using AccessControlApi.Facade;
using AccessControlApi.Common;
using AccessControlApi.Dto.Source;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AccessControlApi.Test;

public class DoorTest
{
    private readonly Mock<IDoorRepository> _doorRepositoryMock;
    private readonly Mock<RoleManager<Role>> _roleManagerMock;
    private readonly Mock<ClaimsPrincipal> _principalMock;
    private readonly Mock<HttpContext> _httpContextMock;

    public DoorTest()
    {
        _doorRepositoryMock = new Mock<IDoorRepository>();
        _roleManagerMock = new Mock<RoleManager<Role>>(Mock.Of<IRoleStore<Role>>(), null, null, null, null);
        _principalMock = new Mock<ClaimsPrincipal>();
        _httpContextMock = new Mock<HttpContext>();
    }

    [Fact]
    public async void Add()
    {
        var door = new Door
        {
            Id = "54cd3375-bfef-458f-b450-463d16418ffa",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Main entrance"
        };

        _doorRepositoryMock.Setup(d => d.Add(It.IsAny<Door>()))
            .ReturnsAsync(door);

        var doorFacade = new DoorFacade(_doorRepositoryMock.Object, _roleManagerMock.Object);
        var doorController = new DoorController(doorFacade);

        var addDoorRequest = new AddDoorRequest { Name = door.Name };

        var expectedAddDoorResponse = new AddDoorResponse(door);

        var actionResult = await doorController.Add(addDoorRequest);
        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var operationResult = Assert.IsAssignableFrom<OperationResult<AddDoorResponse>>(response.Value);

        Assert.NotNull(response);
        Assert.True(operationResult.Success);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedAddDoorResponse),
            JsonConvert.SerializeObject(operationResult.Data));
    }

    [Fact]
    public async void AssignRole()
    {
        var door = new Door
        {
            Id = "54cd3375-bfef-458f-b450-463d16418ffa",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Main entrance"
        };

        var role = new Role
        {
            Id = "e946edc2-ddeb-41fc-9210-39b8a6b03314",
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = "fa5a4370-01bd-4eeb-9218-94320d8c581f"
        };

        var doorRole = new DoorRole
        {
            Door = door,
            DoorId = door.Id,
            Role = role,
            RoleId = role.Id
        };

        _doorRepositoryMock.Setup(d => d.Find(It.IsAny<string>()))
            .ReturnsAsync(door);

        _doorRepositoryMock.Setup(d => d.IsDoorRoleExists(It.IsAny<DoorRole>()))
            .ReturnsAsync(false);

        _roleManagerMock.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(role);

        _doorRepositoryMock.Setup(d => d.AssignRole(It.IsAny<DoorRole>()))
            .ReturnsAsync(doorRole);

        var doorFacade = new DoorFacade(_doorRepositoryMock.Object, _roleManagerMock.Object);
        var doorController = new DoorController(doorFacade);

        var assignRoleToDoorRequest = new AssignRoleToDoorRequest { DoorId = door.Id, RoleId = role.Id};

        var expectedAssignRoleToDoorResponse = new AssignRoleToDoorResponse(doorRole);

        var actionResult = await doorController.AssignRole(assignRoleToDoorRequest);
        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var operationResult = Assert.IsAssignableFrom<OperationResult<AssignRoleToDoorResponse>>(response.Value);

        Assert.NotNull(response);
        Assert.True(operationResult.Success);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedAssignRoleToDoorResponse),
            JsonConvert.SerializeObject(operationResult.Data));
    }

    [Fact]
    public async void Open()
    {
        var door = new Door
        {
            Id = "54cd3375-bfef-458f-b450-463d16418ffa",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Main entrance"
        };

        User user = new()
        {
            Id = "f8389291-a097-4507-80dc-663790c1d971",
            UserName = "berkayopak",
            Email = "berkayopak@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var doorEventHistory = new DoorEventHistory
        {
            Id = 1,
            AttemptedAt = DateTime.Now,
            Door = door,
            DoorId = door.Id,
            EventType = DoorEventType.Open,
            Information = null,
            Succeeded = true,
            User = user,
            UserId = user.Id
        };

        var claimList = new List<Claim>
        {
            new(ClaimTypes.Role, RoleName.SuperAdmin)
        };

        _principalMock.Setup(p => p.FindAll(It.IsAny<string>()))
            .Returns(claimList);

        _httpContextMock.Setup(h => h.User)
            .Returns(_principalMock.Object);

        _doorRepositoryMock.Setup(d => d.Find(It.IsAny<string>()))
            .ReturnsAsync(door);

        _doorRepositoryMock.Setup(d =>
                d.HasDoorAnyMatchingRole(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<string>>()))
            .ReturnsAsync(true);

        _doorRepositoryMock.Setup(d => d.AddEventToHistory(It.IsAny<DoorEventHistory>()))
            .ReturnsAsync(doorEventHistory);

        var doorFacade = new DoorFacade(_doorRepositoryMock.Object, _roleManagerMock.Object);
        var doorController = new DoorController(doorFacade)
        {
            ControllerContext =
            {
                HttpContext = _httpContextMock.Object
            }
        };

        var openDoorRequest = new OpenDoorRequest { DoorId = door.Id };

        var expectedOpenDoorResponse = new OpenDoorResponse(door.Id, true);

        var actionResult = await doorController.Open(openDoorRequest);
        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var operationResult = Assert.IsAssignableFrom<OperationResult<OpenDoorResponse>>(response.Value);

        Assert.NotNull(response);
        Assert.True(operationResult.Success);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedOpenDoorResponse),
            JsonConvert.SerializeObject(operationResult.Data));
    }

    [Fact]
    public async void GetEventHistory()
    {
        var door = new Door
        {
            Id = "54cd3375-bfef-458f-b450-463d16418ffa",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Main entrance"
        };

        User user = new()
        {
            Id = "f8389291-a097-4507-80dc-663790c1d971",
            UserName = "berkayopak",
            Email = "berkayopak@gmail.com",
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var doorEventHistory = new DoorEventHistory
        {
            Id = 1,
            AttemptedAt = DateTime.Now,
            Door = door,
            DoorId = door.Id,
            EventType = DoorEventType.Open,
            Information = null,
            Succeeded = true,
            User = user,
            UserId = user.Id
        };

        var doorEventHistoryList = new List<DoorEventHistory> { doorEventHistory };

        _doorRepositoryMock.Setup(d => d.GetDoorEventHistoryCount(It.IsAny<GetDoorEventHistoryFilter>()))
            .ReturnsAsync(doorEventHistoryList.Count);

        _doorRepositoryMock.Setup(d => d.GetDoorEventHistory(It.IsAny<GetDoorEventHistoryFilter>()))
            .ReturnsAsync(doorEventHistoryList);

        var doorFacade = new DoorFacade(_doorRepositoryMock.Object, _roleManagerMock.Object);
        var doorController = new DoorController(doorFacade);

        var getDoorEventHistoryRequest = new GetDoorEventHistoryRequest { PageNumber = 1, PageSize = 10 };

        var expectedGetDoorEventHistoryResponse = new GetDoorEventHistoryResponse(doorEventHistoryList);
        var totalItemCount = doorEventHistoryList.Count;
        var totalPageCount = (int)Math.Ceiling((double)totalItemCount / getDoorEventHistoryRequest.PageSize);

        var expectedPaginatedGetDoorEventHistoryResponse =
            new BasePaginationResponse<GetDoorEventHistoryResponse>(
                expectedGetDoorEventHistoryResponse,
                totalItemCount,
                totalPageCount,
                getDoorEventHistoryRequest.PageNumber,
                getDoorEventHistoryRequest.PageSize);

        var actionResult = await doorController.GetEventHistory(getDoorEventHistoryRequest);
        var response = Assert.IsType<OkObjectResult>(actionResult.Result);
        var operationResult = Assert.IsAssignableFrom<OperationResult<BasePaginationResponse<GetDoorEventHistoryResponse>>>(response.Value);

        Assert.NotNull(response);
        Assert.True(operationResult.Success);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(JsonConvert.SerializeObject(expectedPaginatedGetDoorEventHistoryResponse),
            JsonConvert.SerializeObject(operationResult.Data));
    }
}