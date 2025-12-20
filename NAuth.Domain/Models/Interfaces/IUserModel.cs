using NAuth.Domain.Enums;
using NAuth.Domain.Factory.Interfaces;
using System;
using System.Collections.Generic;

namespace NAuth.Domain.Models.Models
{
    public interface IUserModel
    {
        long UserId { get; set; }
        string Hash { get; set; }
        string Slug { get; set; }
        string Image { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string IdDocument { get; set; }
        string PixKey { get; set; }
        DateTime? BirthDate { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        bool IsAdmin { get; set; }
        string StripeId { get; set; }
        UserStatus Status { get; set; }

        IUserModel Insert(IUserDomainFactory factory);
        IUserModel Update(IUserDomainFactory factory);
        IUserModel GetByEmail(string email, IUserDomainFactory factory);
        IUserModel GetBySlug(string slug, IUserDomainFactory factory);
        IUserModel GetById(long userId, IUserDomainFactory factory);
        IUserModel GetByStripeId(string stripeId, IUserDomainFactory factory);
        IUserModel GetByRecoveryHash(string recoveryHash, IUserDomainFactory factory);
        IEnumerable<IUserModel> ListUsers(IUserDomainFactory factory);
        IEnumerable<IUserModel> SearchUsers(string searchTerm, int page, int pageSize, out int totalCount, IUserDomainFactory factory);
        IUserModel LoginWithEmail(string email, string password, IUserDomainFactory factory);
        bool HasPassword(long userId, IUserDomainFactory factory);
        void ChangePassword(long userId, string password, IUserDomainFactory factory);
        void ChangePasswordUsingHash(string recoveryHash, string password, IUserDomainFactory factory);
        string GenerateRecoveryHash(long userId, IUserDomainFactory factory);
        bool ExistSlug(long userId, string slug);
        IEnumerable<IRoleModel> ListRoles(long userId, IRoleDomainFactory roleFactory);
        void AddRole(long userId, long roleId);
        void RemoveRole(long userId, long roleId);
        void RemoveAllRoles(long userId);
        bool HasRole(long userId, long roleId);
        bool HasRoleBySlug(long userId, string roleSlug);
    }
}
