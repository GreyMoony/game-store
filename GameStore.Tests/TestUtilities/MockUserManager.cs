using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.TestUtilities;
public static class MockUserManager
{
    public static Mock<UserManager<TUser>> CreateUserManager<TUser>()
        where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

        return userManager;
    }

    public static Mock<RoleManager<IdentityRole>> CreateRoleManager()
    {
        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        var roleValidators = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errorDescriber = new IdentityErrorDescriber();
        var logger = new Mock<ILogger<RoleManager<IdentityRole>>>();

        return new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object,
            roleValidators,
            keyNormalizer.Object,
            errorDescriber,
            logger.Object);
    }
}
