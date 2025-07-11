using Microsoft.AspNetCore.Http;
using NAuth.DTO.Domain;
using NAuth.DTO.User;
using System.IO;
using System.Threading.Tasks;

namespace NAuth.Client
{
    public interface IUserClient
    {
        void SetApiURL(string apiURL);
        UserInfo? GetUserInSession(HttpContext httpContext);
        Task<UserTokenResult?> GetTokenAuthorizedAsync(LoginParam login);
        Task<UserResult?> GetMeAsync(string token);
        Task<UserResult?> GetByIdAsync(long userId);
        Task<UserResult?> GetByTokenAsync(string token);
        Task<UserResult?> GetByEmailAsync(string email);
        Task<UserResult?> GetBySlugAsync(string slug);
        Task<UserResult?> InsertAsync(UserInfo user);
        Task<UserResult?> UpdateAsync(UserInfo user, string token);
        Task<UserResult?> LoginWithEmailAsync(LoginParam param);
        Task<StatusResult?> HasPasswordAsync(string token);
        Task<StatusResult?> ChangePasswordAsync(ChangePasswordParam param, string token);
        Task<StatusResult?> SendRecoveryMailAsync(string email);
        Task<StatusResult?> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param);
        Task<UserListResult?> ListAsync(int take);
        Task<StringResult?> UploadImageUserAsync(Stream fileStream, string fileName, string token);
    }
}