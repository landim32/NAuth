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
        IUserModel LoginWithEmail(string email, string password);
        bool HasPassword(long userId);
        void ChangePasswordUsingHash(string recoveryHash, string newPassword);
        void ChangePassword(long userId, string oldPassword, string newPassword);
        Task<bool> SendRecoveryEmail(string email);

        IUserModel Insert(UserInfo user);
        IUserModel Update(UserInfo user);
        IUserModel GetUserByEmail(string email);
        IUserModel GetBySlug(string slug);
        IUserModel GetUserByID(long userId);
        IUserModel GetUserByToken(string token);
        IUserModel GetByStripeId(string stripeId);
        //IUserModel GetUserHash(ChainEnum chain, string address);
        UserInfo GetUserInSession(HttpContext httpContext);
        UserInfo GetUserInfoFromModel(IUserModel md);
        IList<IUserModel> ListUsers(int take);
    }
}
