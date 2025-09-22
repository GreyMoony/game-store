using AutoMapper;
using GameStore.API.Controllers;
using GameStore.API.Mappings;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.UserServices;
using GameStore.Tests.TestUtilities.ControllersTestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests.ControllersTests;
public class RolesControllerTests
{
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly RolesController _controller;

    public RolesControllerTests()
    {
        _mockRoleService = new Mock<IRoleService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        _controller = new RolesController(_mockRoleService.Object, mapper);
    }

    [Fact]
    public async Task GetAllRoles_ShouldReturnListOfRoles()
    {
        // Arrange
        var roles = RolesControllerTestData.RoleDtos;
        _mockRoleService.Setup(s => s.GetAllAsync()).ReturnsAsync(roles);

        // Act
        var result = await _controller.GetAllRoles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(roles, okResult.Value);
    }

    [Fact]
    public async Task GetRoleById_ShouldReturnOk_WithRole()
    {
        // Arrange
        var role = new RoleDto { Id = "1", Name = "Admin" };
        _mockRoleService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(role);

        // Act
        var result = await _controller.GetRoleById("1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(role, okResult.Value);
    }

    [Fact]
    public async Task DeleteRole_WhenSuccess_ShouldReturnNoContent()
    {
        // Arrange
        var deleteResult = IdentityResult.Success;
        _mockRoleService.Setup(s => s.DeleteRoleAsync("1")).ReturnsAsync(deleteResult);

        // Act
        var result = await _controller.DeleteRole("1");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRole_WhenFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var deleteResult = IdentityResult.Failed(new IdentityError { Description = "Error deleting role" });
        _mockRoleService.Setup(s => s.DeleteRoleAsync("1")).ReturnsAsync(deleteResult);

        // Act
        var result = await _controller.DeleteRole("1");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetUserRoles_ShouldReturnOkWithUserRoles()
    {
        // Arrange
        var roles = RolesControllerTestData.RoleDtos;
        _mockRoleService.Setup(s => s.GetUsersRolesAsync("1")).ReturnsAsync(roles);

        // Act
        var result = await _controller.GetUserRoles("1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(roles, okResult.Value);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnOkWithPermissions()
    {
        // Arrange
        var permissions = RolesControllerTestData.Permissions;
        _mockRoleService.Setup(s => s.GetAllPermissionsAsync()).ReturnsAsync(permissions);

        // Act
        var result = await _controller.GetPermissions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(permissions, okResult.Value);
    }

    [Fact]
    public async Task GetRolePermissions_ShouldReturnOk_WithRolePermissions()
    {
        // Arrange
        var permissions = RolesControllerTestData.Permissions;
        _mockRoleService.Setup(s => s.GetPermissionsByRoleIdAsync("1")).ReturnsAsync(permissions);

        // Act
        var result = await _controller.GetRolePermissions("1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(permissions, okResult.Value);
    }

    [Fact]
    public async Task CreateRole_ShouldReturnCreatedAtAction_WithRole()
    {
        // Arrange
        var request = RolesControllerTestData.CreateRoleRequest;
        var createdRole = new RoleDto { Id = "2", Name = "Manager" };

        _mockRoleService.Setup(s => s.CreateRoleAsync(It.IsAny<CreateRoleDto>())).ReturnsAsync(createdRole);

        // Act
        var result = await _controller.CreateRole(request);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(createdRole, okResult.Value);
    }

    [Fact]
    public async Task UpdateRole_ShouldReturnNoContent()
    {
        // Arrange
        var request = RolesControllerTestData.UpdateRoleRequest;

        _mockRoleService.Setup(s => s.UpdateRoleAsync(It.IsAny<UpdateRoleDto>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateRole(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
