using System;
using Moq;
using Xunit;
using NAuth.Domain.Impl.Services;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using NAuth.Domain.Interfaces.Services;

namespace NAuth.Test;

public class UserServiceTests
{
    private UserService CreateService(
        Mock<IUserDomainFactory>? userFactory = null,
        Mock<IUserTokenDomainFactory>? tokenFactory = null,
        Mock<IUserPhoneDomainFactory>? phoneFactory = null,
        Mock<IUserAddressDomainFactory>? addrFactory = null,
        Mock<IMailerSendService>? mailer = null,
        Mock<IImageService>? image = null)
    {
        userFactory ??= new Mock<IUserDomainFactory>();
        tokenFactory ??= new Mock<IUserTokenDomainFactory>();
        phoneFactory ??= new Mock<IUserPhoneDomainFactory>();
        addrFactory ??= new Mock<IUserAddressDomainFactory>();
        mailer ??= new Mock<IMailerSendService>();
        image ??= new Mock<IImageService>();

        return new UserService(
            userFactory.Object,
            phoneFactory.Object,
            addrFactory.Object,
            tokenFactory.Object,
            mailer.Object,
            image.Object);
    }

    [Theory]
    [InlineData(0, "127.0.0.1", "UA", "fp")]
    [InlineData(1, "", "UA", "fp")]
    [InlineData(1, "127.0.0.1", "", "fp")]
    [InlineData(1, "127.0.0.1", "UA", "")]
    public void CreateToken_InvalidArguments_ThrowsException(long userId, string ip, string ua, string fingerprint)
    {
        var service = CreateService();
        Assert.Throws<Exception>(() => service.CreateToken(userId, ip, ua, fingerprint));
    }

    [Fact]
    public void CreateToken_ValidArguments_CallsInsertAndReturnsModel()
    {
        var tokenModel = new Mock<IUserTokenModel>();
        tokenModel.SetupAllProperties();
        var tokenFactory = new Mock<IUserTokenDomainFactory>();
        tokenFactory.Setup(f => f.BuildUserTokenModel()).Returns(tokenModel.Object);
        tokenModel.Setup(m => m.Insert(tokenFactory.Object)).Returns(tokenModel.Object);

        var service = CreateService(tokenFactory: tokenFactory);

        var result = service.CreateToken(1, "127.0.0.1", "UA", "fp");

        tokenFactory.Verify(f => f.BuildUserTokenModel(), Times.Once);
        tokenModel.Verify(m => m.Insert(tokenFactory.Object), Times.Once);
        Assert.Equal(1, result.UserId);
        Assert.Equal("127.0.0.1", result.IpAddress);
        Assert.Equal("UA", result.UserAgent);
        Assert.Equal("fp", result.Fingerprint);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public void ChangePasswordUsingHash_UserNotFound_ThrowsException()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);
        userModel.Setup(m => m.GetByRecoveryHash("hash", userFactory.Object)).Returns((IUserModel?)null);

        var service = CreateService(userFactory: userFactory);

        Assert.Throws<Exception>(() => service.ChangePasswordUsingHash("hash", "newpass"));
    }

    [Fact]
    public void ChangePasswordUsingHash_Valid_CallsChangePassword()
    {
        var userModel = new Mock<IUserModel>();
        var userFactory = new Mock<IUserDomainFactory>();
        userFactory.Setup(f => f.BuildUserModel()).Returns(userModel.Object);

        var existingUser = new Mock<IUserModel>();
        existingUser.SetupGet(u => u.UserId).Returns(10);
        userModel.Setup(m => m.GetByRecoveryHash("hash", userFactory.Object)).Returns(existingUser.Object);

        var service = CreateService(userFactory: userFactory);

        service.ChangePasswordUsingHash("hash", "newpass");

        userModel.Verify(m => m.ChangePassword(10, "newpass", userFactory.Object), Times.Once);
    }
}

