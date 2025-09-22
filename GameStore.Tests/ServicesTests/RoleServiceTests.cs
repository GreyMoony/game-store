using System.Security.Claims;
using GameStore.Application.Services.UserServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MockQuery;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class RoleServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly RoleService _service;

    public RoleServiceTests()
    {
        _userManagerMock = MockUserManager.CreateUserManager<ApplicationUser>();
        _roleManagerMock = MockUserManager.CreateRoleManager();
        _service = new RoleService(_userManagerMock.Object, _roleManagerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleExists_ShouldReturnRole()
    {
        // Arrange
        var role = RoleServiceTestData.Role;
        _roleManagerMock.Setup(r => r.FindByIdAsync(role.Id))
            .ReturnsAsync(role);

        // Act
        var result = await _service.GetByIdAsync(role.Id);

        // Assert
        Assert.Equal(role.Id, result.Id);
        Assert.Equal(role.Name, result.Name);
    }

    [Fact]
    public async Task CreateRoleAsync_WhenRoleAlreadyExists_ShouldThrowException()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<UniquePropertyException>(
            () => _service.CreateRoleAsync(RoleServiceTestData.CreateRoleDto));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = new List<IdentityRole>
        {
            RoleServiceTestData.AdminRole,
            RoleServiceTestData.ManagerRole,
        };

        var mockRoleQueryable = roles.AsQueryable().BuildMock();

        _roleManagerMock.Setup(r => r.Roles).Returns(mockRoleQueryable.Object);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "Admin");
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoleNotFound_ShouldThrowException()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((IdentityRole)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.GetByIdAsync("invalid-id"));
    }

    [Fact]
    public async Task DeleteRoleAsync_ShouldDeleteRole_WhenRoleExists()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.FindByIdAsync("role-1"))
            .ReturnsAsync(RoleServiceTestData.AdminRole);
        _roleManagerMock.Setup(r => r.DeleteAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.DeleteRoleAsync("role-1");

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task GetPermissionsByRoleIdAsync_ShouldReturnPermissions_WhenRoleExists()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.FindByIdAsync("role-1"))
            .ReturnsAsync(RoleServiceTestData.AdminRole);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(RoleServiceTestData.AdminClaims);

        // Act
        var result = await _service.GetPermissionsByRoleIdAsync("role-1");

        // Assert
        Assert.Contains("ManageUsers", result);
        Assert.Contains("ViewReports", result);
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldUpdateRoleNameAndPermissions()
    {
        // Arrange
        var role = RoleServiceTestData.AdminRole;
        _roleManagerMock.Setup(r => r.FindByIdAsync("role-1")).ReturnsAsync(role);
        _roleManagerMock.Setup(
            r => r.SetRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(RoleServiceTestData.AdminClaims);
        _roleManagerMock.Setup(r => r.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(r => r.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.UpdateRoleAsync(RoleServiceTestData.UpdateRoleDto);

        // Assert
        _roleManagerMock.Verify(r => r.SetRoleNameAsync(role, "SuperAdmin"), Times.Once);
        _roleManagerMock.Verify(r => r.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
        _roleManagerMock.Verify(r => r.AddClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateRoleAsync_FailedNameSetting_ShouldThrowDbUpdateException()
    {
        // Arrange
        var role = RoleServiceTestData.AdminRole;
        _roleManagerMock.Setup(r => r.FindByIdAsync("role-1")).ReturnsAsync(role);
        _roleManagerMock.Setup(
            r => r.SetRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        // Act/Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _service.UpdateRoleAsync(RoleServiceTestData.UpdateRoleDto));
    }

    [Fact]
    public async Task GetUsersRolesAsync_WhenUserExists_ShouldReturnRoles()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(RoleServiceTestData.UserWithRoles);
        _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Admin", "Manager"]);
        _roleManagerMock.Setup(r => r.FindByNameAsync("Admin"))
            .ReturnsAsync(RoleServiceTestData.AdminRole);
        _roleManagerMock.Setup(r => r.FindByNameAsync("Manager"))
            .ReturnsAsync(RoleServiceTestData.ManagerRole);

        // Act
        var result = await _service.GetUsersRolesAsync("user-1");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "Admin");
        Assert.Contains(result, r => r.Name == "Manager");
    }

    [Fact]
    public async Task GetUsersRolesAsync_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("invalid-user"))
            .ReturnsAsync((ApplicationUser)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetUsersRolesAsync("invalid-user"));
    }

    [Fact]
    public async Task GetAllPermissionsAsync_ShouldReturnAllPermissions()
    {
        // Arrange
        var roles = new List<IdentityRole>
        {
            RoleServiceTestData.AdminRole,
            RoleServiceTestData.ManagerRole,
        };
        var mockRoleQueryable = roles.AsQueryable().BuildMock();

        _roleManagerMock.Setup(r => r.Roles).Returns(mockRoleQueryable.Object);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync((IdentityRole role) =>
            role.Id == "role-1" ?
            RoleServiceTestData.AdminClaims :
            []);

        // Act
        var result = await _service.GetAllPermissionsAsync();

        // Assert
        Assert.Contains("ManageUsers", result);
        Assert.Contains("ViewReports", result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateRoleAsync_WhenValid_ShouldCreateRole()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.RoleExistsAsync(RoleServiceTestData.CreateRoleDto.Role.Name))
            .ReturnsAsync(false);
        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(r => r.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.CreateRoleAsync(RoleServiceTestData.CreateRoleDto);

        // Assert
        Assert.Equal(RoleServiceTestData.CreateRoleDto.Role.Name, result.Name);
        _roleManagerMock.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        _roleManagerMock.Verify(r => r.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldThrowException_WhenCreationFails()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.RoleExistsAsync(RoleServiceTestData.CreateRoleDto.Role.Name))
            .ReturnsAsync(false);
        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

        // Act & Assert
        await Assert.ThrowsAsync<EntityCreationException>(
            () => _service.CreateRoleAsync(RoleServiceTestData.CreateRoleDto));
    }
}
