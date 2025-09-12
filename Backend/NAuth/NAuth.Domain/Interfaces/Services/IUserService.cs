using System;
using System.Collections.Generic;
using NAuth.Domain.Interfaces.Models;
using NAuth.DTO.User;
using Microsoft.AspNetCore.Http;
using NAuth.Domain.Impl.Models;
using System.Threading.Tasks;

namespace NAuth.Domain.Interfaces.Services
{
    public interface IUserService
    {
        string GetBucketName();
        IUserModel LoginWithEmail(string email, string password);
        Task<IUserTokenModel> CreateToken(long userId, string ipAddress, string userAgent, string fingerprint);
        bool HasPassword(long userId);
        void ChangePasswordUsingHash(string recoveryHash, string newPassword);
        void ChangePassword(long userId, string oldPassword, string newPassword);
        Task<bool> SendRecoveryEmail(string email);

        Task<IUserModel> Insert(UserInfo user);
        Task<IUserModel> Update(UserInfo user);
        IUserModel GetUserByEmail(string email);
        IUserModel GetBySlug(string slug);
        IUserModel GetUserByID(long userId);
        IUserModel GetUserByToken(string token);
        IUserModel GetByStripeId(string stripeId);
        UserInfo GetUserInSession(HttpContext httpContext);
        Task<UserInfo> GetUserInfoFromModel(IUserModel md);
        IList<IUserModel> ListUsers(int take);
    }
}
