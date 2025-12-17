using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using System;
using System.Collections.Generic;
using BCrypt.Net;

namespace NAuth.Domain.Models
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base(UserModel.UserNotFoundMessage) { }
    }

    public class UserModel : IUserModel
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserRepository<IUserModel, IUserDomainFactory> _repositoryUser;
        protected readonly IUserRoleRepository<IRoleModel, IRoleDomainFactory> _repositoryUserRole;

        public const string UserNotFoundMessage = "User not found";

        public UserModel(
            IUnitOfWork unitOfWork, 
            IUserRepository<IUserModel, IUserDomainFactory> repositoryUser,
            IUserRoleRepository<IRoleModel, IRoleDomainFactory> repositoryUserRole
        )
        {
            _unitOfWork = unitOfWork;
            _repositoryUser = repositoryUser;
            _repositoryUserRole = repositoryUserRole;
        }

        public long UserId { get; set; }
        public string Hash { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IdDocument { get; set; }
        public string PixKey { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string StripeId { get; set; }

        public IUserModel GetById(long userId, IUserDomainFactory factory)
        {
            return _repositoryUser.GetById(userId, factory);
        }

        private string HashPassword(string password)
        {
            // BCrypt automaticamente gera um salt e usa work factor de 12 (padrão)
            // Isso torna o hashing mais lento e seguro contra ataques de força bruta
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // BCrypt compara a senha com o hash de forma segura
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public IUserModel Insert(IUserDomainFactory factory)
        {
            return _repositoryUser.Insert(this, factory);
        }

        public IUserModel Update(IUserDomainFactory factory)
        {
            return _repositoryUser.Update(this, factory);
        }

        public IEnumerable<IUserModel> ListUsers(int take, IUserDomainFactory factory)
        {
            return _repositoryUser.ListUsers(take, factory);
        }

        public IUserModel GetByEmail(string email, IUserDomainFactory factory)
        {
            return _repositoryUser.GetByEmail(email, factory);
        }
        public IUserModel GetBySlug(string slug, IUserDomainFactory factory)
        {
            return _repositoryUser.GetBySlug(slug, factory);
        }

        public IUserModel GetByRecoveryHash(string recoveryHash, IUserDomainFactory factory)
        {
            return _repositoryUser.GetUserByRecoveryHash(recoveryHash, factory);
        }

        public IUserModel LoginWithEmail(string email, string password, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetByEmail(email, factory);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            var hashPassword = _repositoryUser.GetHashedPassword(user.UserId);
            if (string.IsNullOrEmpty(hashPassword))
            {
                return null; // Usuário não tem senha definida
            }

            // Usa verificação BCrypt direta
            if (!VerifyPassword(password, hashPassword))
            {
                return null; // Senha incorreta
            }

            return user;
        }

        public bool HasPassword(long userId, IUserDomainFactory factory)
        {
            return _repositoryUser.HasPassword(userId, factory);
        }

        public void ChangePassword(long userId, string password, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetById(userId, factory);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            
            // Usa BCrypt para criar o hash da nova senha
            string hashedPassword = HashPassword(password);
            _repositoryUser.ChangePassword(userId, hashedPassword);
        }

        public void ChangePasswordUsingHash(string recoveryHash, string password, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetUserByRecoveryHash(recoveryHash, factory);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            
            // Usa BCrypt para criar o hash da nova senha
            string hashedPassword = HashPassword(password);
            _repositoryUser.ChangePassword(user.UserId, hashedPassword);
        }

        public string GenerateRecoveryHash(long userId, IUserDomainFactory factory)
        {
            var user = _repositoryUser.GetById(userId, factory);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            
            // Gera um token seguro para recuperação de senha usando BCrypt
            string recoveryToken = Guid.NewGuid().ToString() + user.UserId + DateTime.UtcNow.Ticks;
            string recoveryHash = HashPassword(recoveryToken);
            _repositoryUser.UpdateRecoveryHash(userId, recoveryHash);
            return recoveryHash;
        }

        public bool ExistSlug(long userId, string slug)
        {
            return _repositoryUser.ExistSlug(userId, slug);
        }

        public IUserModel GetByStripeId(string stripeId, IUserDomainFactory factory)
        {
            return _repositoryUser.GetByStripeId(stripeId, factory);
        }

        public IEnumerable<IRoleModel> ListRoles(long userId, IRoleDomainFactory roleFactory)
        {
            return _repositoryUserRole.ListRolesByUser(userId, roleFactory);
        }

        public void AddRole(long userId, long roleId)
        {
            _repositoryUserRole.AddRoleToUser(userId, roleId);
        }

        public void RemoveRole(long userId, long roleId)
        {
            _repositoryUserRole.RemoveRoleFromUser(userId, roleId);
        }

        public void RemoveAllRoles(long userId)
        {
            _repositoryUserRole.RemoveAllRolesFromUser(userId);
        }

        public bool HasRole(long userId, long roleId)
        {
            return _repositoryUserRole.UserHasRole(userId, roleId);
        }

        public bool HasRoleBySlug(long userId, string roleSlug)
        {
            return _repositoryUserRole.UserHasRoleBySlug(userId, roleSlug);
        }
    }
}
