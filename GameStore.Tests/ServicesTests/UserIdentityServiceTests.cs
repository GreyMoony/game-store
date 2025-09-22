using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.UserServices;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Constants;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Enums;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Settings;
using GameStore.Tests.TestUtilities;
using GameStore.Tests.TestUtilities.MockQuery;
using GameStore.Tests.TestUtilities.ServicesTestData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace GameStore.Tests.ServicesTests;
public class UserIdentityServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IExternalAuthService> _authServiceMock;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkMongo> _unitOfWorkMongoMock;
    private readonly UserIdentityService _service;

    public UserIdentityServiceTests()
    {
        _userManagerMock = MockUserManager.CreateUserManager<ApplicationUser>();
        _roleManagerMock = MockUserManager.CreateRoleManager();
        _authServiceMock = new Mock<IExternalAuthService>();
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(j => j.Value).Returns(
            new JwtSettings { Secret = "YourSuperSecureSecretKeyThatIsAtLeast32CharactersLong" });
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMongoMock = new Mock<IUnitOfWorkMongo>();

        _service = new UserIdentityService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _authServiceMock.Object,
            _jwtSettingsMock.Object,
            _unitOfWorkMock.Object,
            _unitOfWorkMongoMock.Object);
    }

    [Fact]
    public async Task Login_WhenValidCredentials_ShouldReturnToken()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByNameAsync("AdminUser"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(
            It.IsAny<ApplicationUser>(), "CorrectPassword"))
            .ReturnsAsync(true);
        _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Admin"]);
        _roleManagerMock.Setup(r => r.FindByNameAsync("Admin"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminRole);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(UserIdentityServiceTestData.AdminClaims);

        // Act
        var token = await _service.Login(UserIdentityServiceTestData.ValidLoginDto);

        // Assert
        Assert.NotNull(token);
    }

    [Fact]
    public async Task Login_WhenValidCredentialsExternal_ShouldReturnToken()
    {
        // Arrange
        _authServiceMock.Setup(a => a.AuthenticateAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(UserIdentityServiceTestData.SuccessAuthentication);
        _roleManagerMock.Setup(r => r.FindByNameAsync(UserRoles.Guest))
            .ReturnsAsync(UserIdentityServiceTestData.GuestRole);
        _roleManagerMock.Setup(r => r.GetClaimsAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(UserIdentityServiceTestData.GuestClaims);

        // Act
        var token = await _service.Login(UserIdentityServiceTestData.ValidExternalLoginDto);

        // Assert
        Assert.NotNull(token);
    }

    [Fact]
    public async Task Login_WhenInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByNameAsync("AdminUser"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), "WrongPassword"))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.Login(UserIdentityServiceTestData.InvalidLoginDto));
    }

    [Fact]
    public async Task Login_ExternalAuthWithInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        _authServiceMock.Setup(a => a.AuthenticateAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(new AuthResult { Success = false });

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.Login(UserIdentityServiceTestData.InvalidExternalLoginDto));
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            UserIdentityServiceTestData.AdminUser,
            UserIdentityServiceTestData.NormalUser,
        };
        var mockUsersQueryable = users.AsQueryable().BuildMock();

        _userManagerMock.Setup(u => u.Users).Returns(mockUsersQueryable.Object);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);

        // Act
        var result = await _service.GetUserByIdAsync("user-1");

        // Assert
        Assert.Equal("AdminUser", result.Name);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("invalid-user"))
            .ReturnsAsync((ApplicationUser)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.GetUserByIdAsync("invalid-user"));
    }

    [Fact]
    public async Task DeleteUserByIdAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.DeleteUserByIdAsync("user-1");

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_WhenUserNotFound_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("invalid-user"))
            .ReturnsAsync((ApplicationUser)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.DeleteUserByIdAsync("invalid-user"));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUserDetails()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.SetUserNameAsync(
            It.IsAny<ApplicationUser>(),
            UserIdentityServiceTestData.UpdateUserDto.User.Name))
        .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        _userManagerMock.Setup(u => u.ResetPasswordAsync(
            It.IsAny<ApplicationUser>(),
            "token",
            UserIdentityServiceTestData.UpdateUserDto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(r => r.FindByIdAsync(UserIdentityServiceTestData.UpdateUserDto.Roles.First()))
            .ReturnsAsync(UserIdentityServiceTestData.AdminRole);
        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.UpdateUserAsync(UserIdentityServiceTestData.UpdateUserDto);

        // Assert
        _userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_NotValidName_ShouldThrowUserUpdateException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.SetUserNameAsync(
            It.IsAny<ApplicationUser>(),
            UserIdentityServiceTestData.UpdateUserDto.User.Name))
        .ReturnsAsync(IdentityResult.Failed());

        // Act/Assert
        await Assert.ThrowsAsync<UserUpdateException>(
            () => _service.UpdateUserAsync(UserIdentityServiceTestData.UpdateUserDto));
    }

    [Fact]
    public async Task UpdateUserAsync_NotValidPassword_ShouldThrowUserUpdateException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.SetUserNameAsync(
            It.IsAny<ApplicationUser>(),
            UserIdentityServiceTestData.UpdateUserDto.User.Name))
        .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        _userManagerMock.Setup(u => u.ResetPasswordAsync(
            It.IsAny<ApplicationUser>(),
            "token",
            UserIdentityServiceTestData.UpdateUserDto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(r => r.FindByIdAsync(UserIdentityServiceTestData.UpdateUserDto.Roles.First()))
            .ReturnsAsync((IdentityRole)null);

        // Act/Assert
        await Assert.ThrowsAsync<IdsNotValidException>(
            () => _service.UpdateUserAsync(UserIdentityServiceTestData.UpdateUserDto));
    }

    [Fact]
    public async Task UpdateUserAsync_NotValidRoles_ShouldThrowIdsNotValidException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.FindByIdAsync("user-1"))
            .ReturnsAsync(UserIdentityServiceTestData.AdminUser);
        _userManagerMock.Setup(u => u.SetUserNameAsync(
            It.IsAny<ApplicationUser>(),
            UserIdentityServiceTestData.UpdateUserDto.User.Name))
        .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("token");
        _userManagerMock.Setup(u => u.ResetPasswordAsync(
            It.IsAny<ApplicationUser>(),
            "token",
            UserIdentityServiceTestData.UpdateUserDto.Password))
            .ReturnsAsync(IdentityResult.Failed());

        // Act/Assert
        await Assert.ThrowsAsync<UserUpdateException>(
            () => _service.UpdateUserAsync(UserIdentityServiceTestData.UpdateUserDto));
    }

    [Fact]
    public async Task AddUserAsync_WhenValid_ShouldCreateUser()
    {
        // Arrange
        _userManagerMock.Setup(
            u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManagerMock.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(UserIdentityServiceTestData.UserRole);
        _userManagerMock.Setup(
            u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.AddUserAsync(UserIdentityServiceTestData.CreateUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(UserIdentityServiceTestData.CreateUserDto.User.Name, result.Name);
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
    }

    [Fact]
    public async Task AddUserAsync_WhenUserCreationFails_ShouldThrowException()
    {
        // Arrange
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(UserIdentityServiceTestData.IdentityErrors));

        // Act & Assert
        await Assert.ThrowsAsync<EntityCreationException>(
            () => _service.AddUserAsync(UserIdentityServiceTestData.InvalidCreateUserDto));
    }

    [Theory]
    [MemberData(
        nameof(UserIdentityServiceTestData.CheckAccessCases),
        MemberType = typeof(UserIdentityServiceTestData))]
    public async Task CheckAccess_ShouldReturnExpectedResult(
        CheckAccessDto checkDto, bool includeDeleted, List<UserRole> userRoles, bool expected)
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Games.GetByKeyAsync(It.IsAny<string>(), includeDeleted))
            .ReturnsAsync(expected ? new Game() : null);

        _unitOfWorkMock.Setup(u => u.Genres.GetByIdAsync(It.IsAny<Guid>(), includeDeleted))
            .ReturnsAsync(expected ? new Genre() : null);

        _unitOfWorkMock.Setup(u => u.Publishers.GetByIdAsync(It.IsAny<Guid>(), includeDeleted))
            .ReturnsAsync(expected ? new Publisher() : null);

        _unitOfWorkMock.Setup(u => u.Platforms.GetByIdAsync(It.IsAny<Guid>(), includeDeleted))
            .ReturnsAsync(expected ? new Platform() : null);

        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(It.IsAny<Guid>(), includeDeleted))
            .ReturnsAsync(expected ? new Order() : null);

        _unitOfWorkMock.Setup(u => u.Comments.GetByIdAsync(It.IsAny<Guid>(), includeDeleted))
            .ReturnsAsync(expected ? new Comment() : null);

        _unitOfWorkMongoMock.Setup(u => u.Products.GetByKeyAsync(It.IsAny<string>(), includeDeleted))
            .ReturnsAsync(expected ? new Product() : null);

        _unitOfWorkMongoMock.Setup(u => u.Orders.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(expected ? new OrderNorthwind() : null);

        _unitOfWorkMongoMock.Setup(u => u.Categories.GetByIdAsync(It.IsAny<int>(), includeDeleted))
            .ReturnsAsync(expected ? new Category() : null);

        _unitOfWorkMongoMock.Setup(u => u.Suppliers.GetByIdAsync(It.IsAny<int>(), includeDeleted))
            .ReturnsAsync(expected ? new Supplier() : null);

        // Act
        var result = await _service.CheckAccess(checkDto, includeDeleted, userRoles);

        // Assert
        Assert.Equal(expected, result);
    }
}
