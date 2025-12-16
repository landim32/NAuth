using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Domain.Services.Interfaces;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using Newtonsoft.Json;
using NTools.ACL.Interfaces;
using NTools.DTO.MailerSend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NAuth.Domain.Services
{
    // Definição de exceção customizada para uso em validações de domínio
    [Serializable]
    public class UserValidationException : Exception
    {
        public UserValidationException() { }
        public UserValidationException(string message) : base(message) { }
        public UserValidationException(string message, Exception inner) : base(message, inner) { }
        protected UserValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class UserDomainFactories
    {
        public IUserDomainFactory UserFactory { get; }
        public IUserPhoneDomainFactory PhoneFactory { get; }
        public IUserAddressDomainFactory AddressFactory { get; }
        public IUserTokenDomainFactory TokenFactory { get; }

        public UserDomainFactories(
            IUserDomainFactory userFactory,
            IUserPhoneDomainFactory phoneFactory,
            IUserAddressDomainFactory addressFactory,
            IUserTokenDomainFactory tokenFactory)
        {
            UserFactory = userFactory;
            PhoneFactory = phoneFactory;
            AddressFactory = addressFactory;
            TokenFactory = tokenFactory;
        }
    }

    public class ExternalClients
    {
        public IMailClient MailClient { get; }
        public IFileClient FileClient { get; }
        public IStringClient StringClient { get; }
        public IDocumentClient DocumentClient { get; }

        public ExternalClients(
            IMailClient mailClient,
            IFileClient fileClient,
            IStringClient stringClient,
            IDocumentClient documentClient)
        {
            MailClient = mailClient;
            FileClient = fileClient;
            StringClient = stringClient;
            DocumentClient = documentClient;
        }
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly NAuthSetting _nauthSetting;
        private readonly UserDomainFactories _factories;
        private readonly ExternalClients _clients;

        public UserService(
            ILogger<UserService> logger,
            IOptions<NAuthSetting> nauthSetting,
            UserDomainFactories factories,
            ExternalClients clients)
        {
            _logger = logger;
            _nauthSetting = nauthSetting.Value;
            _factories = factories;
            _clients = clients;
        }

        public string GetBucketName()
        {
            return _nauthSetting.BucketName;
        }

        public IUserModel LoginWithEmail(string email, string password)
        {
            return _factories.UserFactory.BuildUserModel().LoginWithEmail(email, password, _factories.UserFactory);
        }

        public async Task<IUserTokenModel> CreateToken(long userId, string ipAddress, string userAgent, string fingerprint)
        {
            _logger.LogTrace(
                "Creating token for user with ID={@userId}, IP={@ipAddress}, UserAgent={@userAgent} and {@fingerprint}",
                userId, ipAddress, userAgent, fingerprint
            );
            ValidateTokenParameters(userId, ipAddress, userAgent, fingerprint);

            var currentDate = DateTime.Now;
            var tokenModel = _factories.TokenFactory.BuildUserTokenModel();
            tokenModel.UserId = userId;
            tokenModel.IpAddress = ipAddress;
            tokenModel.UserAgent = userAgent;
            tokenModel.Fingerprint = fingerprint;
            tokenModel.CreatedAt = currentDate;
            tokenModel.LastAccess = currentDate;
            tokenModel.ExpireAt = currentDate.AddMonths(2);
            tokenModel.Token = await _clients.StringClient.GenerateShortUniqueStringAsync();

            _logger.LogTrace("Generating token string: {@token} expire at {@expireDate}", tokenModel.Token, tokenModel.ExpireAt);

            return tokenModel.Insert(_factories.TokenFactory);
        }

        private void ValidateTokenParameters(long userId, string ipAddress, string userAgent, string fingerprint)
        {
            if (userId <= 0)
            {
                throw new UserValidationException("UserId is invalid");
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new UserValidationException("IP Address is empty");
            }
            if (string.IsNullOrEmpty(userAgent))
            {
                throw new UserValidationException("User Agent is empty");
            }
            if (string.IsNullOrEmpty(fingerprint))
            {
                throw new UserValidationException("Fingerprint is empty");
            }
        }

        public bool HasPassword(long userId)
        {
            return _factories.UserFactory.BuildUserModel().HasPassword(userId, _factories.UserFactory);
        }

        public void ChangePasswordUsingHash(string recoveryHash, string newPassword)
        {
            if (string.IsNullOrEmpty(recoveryHash))
            {
                throw new UserValidationException("Recovery hash cant be empty");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new UserValidationException("Password cant be empty");
            }

            _logger.LogTrace("Changing password using recovery hash: {@recoveryHash}, new password: {@newPassword}", recoveryHash, newPassword);

            var md = _factories.UserFactory.BuildUserModel();
            var user = md.GetByRecoveryHash(recoveryHash, _factories.UserFactory);
            if (user == null)
            {
                throw new UserValidationException("User not found");
            }
            md.ChangePassword(user.UserId, newPassword, _factories.UserFactory);

            _logger.LogTrace("Password successful changed using recovery hash: {@recoveryHash}", recoveryHash);
        }

        public void ChangePassword(long userId, string oldPassword, string newPassword)
        {
            bool hasPassword = HasPassword(userId);
            if (hasPassword && string.IsNullOrEmpty(oldPassword))
            {
                throw new UserValidationException("Old password cant be empty");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new UserValidationException("New password cant be empty");
            }
            var md = _factories.UserFactory.BuildUserModel();
            var user = md.GetById(userId, _factories.UserFactory);
            if (user == null)
            {
                throw new UserValidationException("User not found");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new UserValidationException("To change password you need a email");
            }
            if (hasPassword)
            {
                var mdUser = md.LoginWithEmail(user.Email, oldPassword, _factories.UserFactory);
                if (mdUser == null)
                {
                    throw new UserValidationException("Email or password is wrong");
                }
            }
            _logger.LogTrace("Changing password using old password: email: {0}, old password: {1}, new password: {2}", user.Email, oldPassword, newPassword);
            md.ChangePassword(user.UserId, newPassword, _factories.UserFactory);
            _logger.LogTrace("Password successful changed using old password");
        }

        public async Task<bool> SendRecoveryEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new UserValidationException("Email cant be empty");
            }
            var md = _factories.UserFactory.BuildUserModel();
            var user = md.GetByEmail(email, _factories.UserFactory);
            if (user == null)
            {
                throw new UserValidationException("User not found");
            }
            var recoveryHash = md.GenerateRecoveryHash(user.UserId, _factories.UserFactory);
            var recoveryUrl = $"https://nochainswap.org/recoverypassword/{recoveryHash}";

            var mail = BuildRecoveryEmail(user, recoveryUrl);
            await _clients.MailClient.SendmailAsync(mail);
            return await Task.FromResult(true);
        }

        private MailerInfo BuildRecoveryEmail(IUserModel user, string recoveryUrl)
        {
            var textMessage =
                $"Hi {user.Name},\r\n\r\n" +
                "We received a request to reset your password. If you made this request, " +
                "please click the link below to reset your password:\r\n\r\n" +
                recoveryUrl + "\r\n\r\n" +
                "If you didn't request a password reset, please ignore this email or contact " +
                "our support team if you have any concerns.\r\n\r\n" +
                "Best regards,\r\n" +
                "NoChainSwap Team";
            var htmlMessage =
                $"Hi <b>{user.Name}</b>,<br />\r\n<br />\r\n" +
                "We received a request to reset your password. If you made this request, " +
                "please click the link below to reset your password:<br />\r\n<br />\r\n" +
                $"<a href=\"{recoveryUrl}\">{recoveryUrl}</a><br />\r\n<br />\r\n" +
                "If you didn't request a password reset, please ignore this email or contact " +
                "our support team if you have any concerns.<br />\r\n<br />\r\n" +
                "Best regards,<br />\r\n" +
                "<b>NoChainSwap Team</b>";

            return new MailerInfo
            {
                From = new MailerRecipientInfo
                {
                    Email = "contact@nochainswap.org",
                    Name = "NoChainSwap Mailmaster"
                },
                To = new List<MailerRecipientInfo> {
                    new MailerRecipientInfo {
                        Email = user.Email,
                        Name = user.Name ?? user.Email
                    }
                },
                Subject = "[NoChainSwap] Password Recovery Email",
                Text = textMessage,
                Html = htmlMessage
            };
        }

        private async Task<string> GenerateSlug(IUserModel md)
        {
            string newSlug;
            int c = 0;
            do
            {
                newSlug = await _clients.StringClient.GenerateSlugAsync(!string.IsNullOrEmpty(md.Slug) ? md.Slug : md.Name);
                if (c > 0)
                {
                    newSlug += c.ToString();
                }
                c++;
            } while (md.ExistSlug(md.UserId, newSlug));
            return newSlug;
        }

        private void InsertPhones(UserInfo user)
        {
            if (user.Phones != null && user.Phones.Count() > 0)
            {
                foreach (var phone in user.Phones)
                {
                    var modelPhone = _factories.PhoneFactory.BuildUserPhoneModel();
                    modelPhone.UserId = user.UserId;
                    modelPhone.Phone = phone.Phone;
                    modelPhone.Insert(_factories.PhoneFactory);
                }
            }
        }

        private void InsertAddresses(UserInfo user)
        {
            if (user.Addresses != null && user.Addresses.Count() > 0)
            {
                foreach (var addr in user.Addresses)
                {
                    var modelAddr = _factories.AddressFactory.BuildUserAddressModel();
                    modelAddr.UserId = user.UserId;
                    modelAddr.ZipCode = addr.ZipCode;
                    modelAddr.Address = addr.Address;
                    modelAddr.Complement = addr.Complement;
                    modelAddr.Neighborhood = addr.Neighborhood;
                    modelAddr.City = addr.City;
                    modelAddr.State = addr.State;
                    modelAddr.Insert(_factories.AddressFactory);
                }
            }
        }

        private async Task ValidatePhones(UserInfo user)
        {
            if (user.Phones == null)
            {
                return;
            }
            foreach (var phone in user.Phones)
            {
                if (string.IsNullOrEmpty(phone.Phone))
                {
                    throw new UserValidationException("Phone is empty");
                }
                else
                {
                    phone.Phone = await _clients.StringClient.OnlyNumbersAsync(phone.Phone.Trim());
                    if (string.IsNullOrEmpty(phone.Phone))
                    {
                        throw new UserValidationException($"{phone.Phone} is not a valid phone");
                    }
                }
            }
        }

        private async Task ValidateAddresses(UserInfo user)
        {
            if (user.Addresses == null)
            {
                return;
            }

            foreach (var addr in user.Addresses)
            {
                ValidateAddressFields(addr);
                await ValidateAndNormalizeZipCode(addr);
            }
        }

        private void ValidateAddressFields(UserAddressInfo addr)
        {
            if (string.IsNullOrEmpty(addr.ZipCode))
            {
                throw new UserValidationException("ZipCode is empty");
            }
            if (string.IsNullOrEmpty(addr.Address))
            {
                throw new UserValidationException("Address is empty");
            }
            if (string.IsNullOrEmpty(addr.Complement))
            {
                throw new UserValidationException("Address is empty");
            }
            if (string.IsNullOrEmpty(addr.Neighborhood))
            {
                throw new UserValidationException("Neighborhood is empty");
            }
            if (string.IsNullOrEmpty(addr.City))
            {
                throw new UserValidationException("City is empty");
            }
            if (string.IsNullOrEmpty(addr.State))
            {
                throw new UserValidationException("State is empty");
            }
        }

        private async Task ValidateAndNormalizeZipCode(UserAddressInfo addr)
        {
            if (!string.IsNullOrEmpty(addr.ZipCode))
            {
                addr.ZipCode = await _clients.StringClient.OnlyNumbersAsync(addr.ZipCode);
                if (string.IsNullOrEmpty(addr.ZipCode))
                {
                    throw new UserValidationException($"{addr.ZipCode} is not a valid zip code");
                }
            }
        }

        public async Task<IUserModel> Insert(UserInfo user)
        {
            var model = _factories.UserFactory.BuildUserModel();
            await ValidateUserForInsert(user, model);

            model.Slug = user.Slug;
            model.Name = user.Name;
            model.Email = user.Email;
            model.BirthDate = user.BirthDate;
            model.IdDocument = user.IdDocument;
            model.PixKey = user.PixKey;
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.Hash = GetUniqueToken();
            model.Slug = await GenerateSlug(model);

            var md = model.Insert(_factories.UserFactory);

            user.UserId = md.UserId;
            InsertPhones(user);
            InsertAddresses(user);

            md.ChangePassword(user.UserId, user.Password, _factories.UserFactory);

            return md;
        }

        private async Task ValidateUserForInsert(UserInfo user, IUserModel model)
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new UserValidationException("Name is empty");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new UserValidationException("Email is empty");
            }
            else
            {
                if (!await _clients.MailClient.IsValidEmailAsync(user.Email))
                {
                    throw new UserValidationException("Email is not valid");
                }
                var userWithEmail = model.GetByEmail(user.Email, _factories.UserFactory);
                if (userWithEmail != null)
                {
                    throw new UserValidationException("User with email already registered");
                }
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new UserValidationException("Password is empty");
            }
            if (!string.IsNullOrEmpty(user.IdDocument))
            {
                user.IdDocument = await _clients.StringClient.OnlyNumbersAsync(user.IdDocument);
                if (!await _clients.DocumentClient.validarCpfOuCnpjAsync(user.IdDocument))
                {
                    throw new UserValidationException($"{user.IdDocument} is not a valid CPF or CNPJ");
                }
            }
            await ValidatePhones(user);
            await ValidateAddresses(user);
        }

        public async Task<IUserModel> Update(UserInfo user)
        {
            IUserModel model = null;
            if (!(user.UserId > 0))
            {
                throw new UserValidationException("User not found");
            }
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new UserValidationException("Name is empty");
            }
            model = _factories.UserFactory.BuildUserModel().GetById(user.UserId, _factories.UserFactory);
            if (model == null)
            {
                throw new UserValidationException("User not exists");
            }
            
            await ValidateUserForUpdate(user, model);

            model.Slug = user.Slug;
            model.Name = user.Name;
            model.Email = user.Email;
            model.BirthDate = user.BirthDate;
            model.IdDocument = user.IdDocument;
            model.PixKey = user.PixKey;
            model.UpdatedAt = DateTime.Now;
            model.Slug = await GenerateSlug(model);

            model.Update(_factories.UserFactory);

            var modelPhone = _factories.PhoneFactory.BuildUserPhoneModel();
            modelPhone.DeleteAllByUser(model.UserId);
            InsertPhones(user);

            var modelAddr = _factories.AddressFactory.BuildUserAddressModel();
            modelAddr.DeleteAllByUser(model.UserId);
            InsertAddresses(user);

            return model;
        }

        private async Task ValidateUserForUpdate(UserInfo user, IUserModel model)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new UserValidationException("Email is empty");
            }
            else
            {
                if (!await _clients.MailClient.IsValidEmailAsync(user.Email))
                {
                    throw new UserValidationException("Email is not valid");
                }
                var userWithEmail = model.GetByEmail(user.Email, _factories.UserFactory);
                if (userWithEmail != null && userWithEmail.UserId != model.UserId)
                {
                    throw new UserValidationException("User with email already registered");
                }
            }
            if (!string.IsNullOrEmpty(user.IdDocument))
            {
                user.IdDocument = await _clients.StringClient.OnlyNumbersAsync(user.IdDocument);
                if (!await _clients.DocumentClient.validarCpfOuCnpjAsync(user.IdDocument))
                {
                    throw new UserValidationException($"{user.IdDocument} is not a valid CPF or CNPJ");
                }
            }
            await ValidatePhones(user);
            await ValidateAddresses(user);
        }

        public IUserModel GetUserByEmail(string email)
        {
            return _factories.UserFactory.BuildUserModel().GetByEmail(email, _factories.UserFactory);
        }

        public IUserModel GetUserByID(long userId)
        {
            return _factories.UserFactory.BuildUserModel().GetById(userId, _factories.UserFactory);
        }

        public IUserModel GetUserByToken(string token)
        {
            var tokenModel = _factories.TokenFactory.BuildUserTokenModel().GetByToken(token, _factories.TokenFactory);
            if (tokenModel == null)
            {
                return null;
            }
            return _factories.UserFactory.BuildUserModel().GetById(tokenModel.UserId, _factories.UserFactory);
        }

        public UserInfo GetUserInSession(HttpContext httpContext)
        {
            if (httpContext.User.Claims.Count() > 0)
            {
                return JsonConvert.DeserializeObject<UserInfo>(httpContext.User.Claims.First().Value);
            }
            return null;
        }

        public async Task<UserInfo> GetUserInfoFromModel(IUserModel md)
        {
            if (md == null)
                return null;
            return new UserInfo
            {
                UserId = md.UserId,
                Hash = md.Hash,
                Slug = md.Slug,
                ImageUrl = await _clients.FileClient.GetFileUrlAsync(GetBucketName(), md.Image),
                Name = md.Name,
                Email = md.Email,
                IdDocument = md.IdDocument,
                PixKey = md.PixKey,
                BirthDate = md.BirthDate,
                CreatedAt = md.CreatedAt,
                UpdatedAt = md.UpdatedAt,
                IsAdmin = md.IsAdmin,
                Phones = _factories.PhoneFactory.BuildUserPhoneModel()
                    .ListByUser(md.UserId, _factories.PhoneFactory)
                    .Select(x => new UserPhoneInfo
                    {
                        Phone = x.Phone
                    }).ToList(),
                Addresses = _factories.AddressFactory.BuildUserAddressModel()
                    .ListByUser(md.UserId, _factories.AddressFactory)
                    .Select(x => new UserAddressInfo
                    {
                        ZipCode = x.ZipCode,
                        Address = x.Address,
                        Complement = x.Complement,
                        Neighborhood = x.Neighborhood,
                        City = x.City,
                        State = x.State
                    }).ToList()
            };
        }

        private string GetUniqueToken()
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
                int length = 100;
                string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
                byte[] data = new byte[length];

                // If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
                // buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
                byte[] buffer = null;

                // Maximum random number that can be used without introducing a bias
                int maxRandom = byte.MaxValue - (byte.MaxValue + 1) % chars.Length;

                crypto.GetBytes(data);

                char[] result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];

                    while (value > maxRandom)
                    {
                        if (buffer == null)
                        {
                            buffer = new byte[1];
                        }

                        crypto.GetBytes(buffer);
                        value = buffer[0];
                    }

                    result[i] = chars[value % chars.Length];
                }

                return new string(result);
            }
        }

        public IUserModel GetByStripeId(string stripeId)
        {
            return _factories.UserFactory.BuildUserModel().GetByStripeId(stripeId, _factories.UserFactory);
        }

        public IUserModel GetBySlug(string slug)
        {
            return _factories.UserFactory.BuildUserModel().GetBySlug(slug, _factories.UserFactory);
        }

        public IList<IUserModel> ListUsers(int take)
        {
            return _factories.UserFactory.BuildUserModel().ListUsers(take, _factories.UserFactory).ToList();
        }
    }
}
