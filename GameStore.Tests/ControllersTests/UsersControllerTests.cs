using System.Security.Claims;
using System.Security.Principal;
using AutoMapper;
using GameStore.API.Controllers;
using GameStore.API.Mappings;
using GameStore.API.Models.UserModels;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.UserServices;
using GameStore.Domain.Constants;
using GameStore.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests.ControllersTests;
public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var notificationServiceMock = new Mock<INotificationService>();
        _controller = new UsersController(
            _userServiceMock.Object, notificationServiceMock.Object, mapper);

        var mockIdentity = new Mock<IIdentity>();
        mockIdentity.Setup(i => i.IsAuthenticated).Returns(true);
        mockIdentity.Setup(i => i.AuthenticationType).Returns("Bearer");

        var mockUser = new Mock<ClaimsPrincipal>();
        mockUser.Setup(u => u.Identity).Returns(mockIdentity.Object);
        mockUser.Setup(u => u.Claims).Returns(
        [
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(Permissions.PermissionClaim, Permissions.ViewDeletedGames),
        ]);
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = mockUser.Object },
        };
    }

    [Fact]
    public async Task Login_WhenLoginIsSuccessful_ShouldReturnToken()
    {
        // Arrange
        var loginRequest = new LoginRequest { Model = new LoginModel { Login = "test", Password = "password" } };
        var token = "fake-token";
        _userServiceMock.Setup(x => x.Login(It.IsAny<LoginDto>())).ReturnsAsync(token);

        // Act
        var result = await _controller.Login(loginRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CheckAccess_WhenUserHasRole_ShouldReturnAccessResult()
    {
        // Arrange
        var checkAccessRequest = new CheckAccessRequest
        {
            TargetId = "testId",
            TargetPage = "TestPage",
        };
        var canSeeDeleted = true;
        var roles = new List<UserRole> { UserRole.Admin };

        _userServiceMock.Setup(x => x.CheckAccess(It.IsAny<CheckAccessDto>(), canSeeDeleted, roles))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckAccess(checkAccessRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task GetAllUsers_WhenAuthorized_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<UserDto> { new() { Id = "1", Name = "testuser" } };
        _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, okResult.Value);
    }

    [Fact]
    public async Task GetUserById_WhenAuthorized_ShouldReturnUser()
    {
        // Arrange
        var userId = "1";
        var user = new UserDto { Id = userId, Name = "testuser" };
        _userServiceMock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public async Task DeleteUser_WhenUserIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        var userId = "1";
        var result = IdentityResult.Success;
        _userServiceMock.Setup(x => x.DeleteUserByIdAsync(userId)).ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task DeleteUser_WhenDeleteFails_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = "1";
        var result = IdentityResult.Failed(new IdentityError { Description = "Error" });
        _userServiceMock.Setup(x => x.DeleteUserByIdAsync(userId)).ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task AddUser_WhenUserIsAdded_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            User = new()
            {
                Name = "NewUser",
            },
            Password = "password",
            Roles =
            [
                "User",
            ],
        };
        var newUser = new UserDto { Id = "2", Name = "NewUser" };
        _userServiceMock.Setup(x => x.AddUserAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(newUser);

        // Act
        var result = await _controller.AddUser(createUserRequest);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenUserIsUpdated()
    {
        // Arrange
        var updateUserRequest = new UpdateUserRequest
        {
            User = new()
            {
                Name = "UpdatedUser",
            },
            Password = "newPassword",
            Roles =
            [
                "User",
            ],
        };
        _userServiceMock.Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserDto>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateUser(updateUserRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}