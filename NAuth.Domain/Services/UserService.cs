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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NAuth.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly NAuthSetting _nauthSetting;
        private readonly IUserDomainFactory _userFactory;
        private readonly IUserPhoneDomainFactory _phoneFactory;
        private readonly IUserAddressDomainFactory _addrFactory;
        private readonly IUserTokenDomainFactory _tokenFactory;
        private readonly IMailClient _mailClient;
        private readonly IFileClient _fileClient;
        private readonly IStringClient _stringClient;
        private readonly IDocumentClient _documentClient;

        public UserService(
            ILogger<UserService> logger,
            IOptions<NAuthSetting> nauthSetting,
            IUserDomainFactory userFactory,
            IUserPhoneDomainFactory phoneFactory,
            IUserAddressDomainFactory addrFactory,
            IUserTokenDomainFactory tokenFactory,
            IMailClient mailClient,
            IFileClient fileClient,
            IStringClient stringClient,
            IDocumentClient documentClient
        )
        {
            _logger = logger;
            _nauthSetting = nauthSetting.Value;
            _userFactory = userFactory;
            _phoneFactory = phoneFactory;
            _addrFactory = addrFactory;
            _tokenFactory = tokenFactory;
            _mailClient = mailClient;
            _fileClient = fileClient;
            _stringClient = stringClient;
            _documentClient = documentClient;
        }

        public string GetBucketName()
        {
            return _nauthSetting.BucketName;
        }

        public IUserModel LoginWithEmail(string email, string password)
        {
            return _userFactory.BuildUserModel().LoginWithEmail(email, password, _userFactory);
        }

        public async Task<IUserTokenModel> CreateToken(long userId, string ipAddress, string userAgent, string fingerprint)
        {
            _logger.LogTrace(
                "Creating token for user with ID={@userId}, IP={@ipAddress}, UserAgent={@userAgent} and {@fingerprint}",
                userId, ipAddress, userAgent, fingerprint
            );
            if (userId <= 0)
            {
                throw new Exception("UserId is invalid");
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new Exception("IP Address is empty");
            }
            if (string.IsNullOrEmpty(userAgent))
            {
                throw new Exception("User Agent is empty");
            }
            if (string.IsNullOrEmpty(fingerprint))
            {
                throw new Exception("Fingerprint is empty");
            }
            var currentDate = DateTime.Now;
            var tokenModel = _tokenFactory.BuildUserTokenModel();
            tokenModel.UserId = userId;
            tokenModel.IpAddress = ipAddress;
            tokenModel.UserAgent = userAgent;
            tokenModel.Fingerprint = fingerprint;
            tokenModel.CreatedAt = currentDate;
            tokenModel.LastAccess = currentDate;
            tokenModel.ExpireAt = currentDate.AddMonths(2);
            tokenModel.Token = await _stringClient.GenerateShortUniqueStringAsync();

            _logger.LogTrace("Generating token string: {@token} expire at {@expireDate}", tokenModel.Token, tokenModel.ExpireAt);

            return tokenModel.Insert(_tokenFactory);
        }

        public bool HasPassword(long userId)
        {
            return _userFactory.BuildUserModel().HasPassword(userId, _userFactory);
        }

        public void ChangePasswordUsingHash(string recoveryHash, string newPassword)
        {
            if (string.IsNullOrEmpty(recoveryHash))
            {
                throw new Exception("Recovery hash cant be empty");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new Exception("Password cant be empty");
            }

            _logger.LogTrace("Changing password using recovery hash: {@recoveryHash}, new password: {@newPassword}", recoveryHash, newPassword);

            var md = _userFactory.BuildUserModel();
            var user = md.GetByRecoveryHash(recoveryHash, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            md.ChangePassword(user.UserId, newPassword, _userFactory);

            _logger.LogTrace("Password successful changed using recovery hash: {@recoveryHash}", recoveryHash);
        }

        public void ChangePassword(long userId, string oldPassword, string newPassword)
        {
            bool hasPassword = HasPassword(userId);
            if (hasPassword && string.IsNullOrEmpty(oldPassword))
            {
                throw new Exception("Old password cant be empty");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new Exception("New password cant be empty");
            }
            var md = _userFactory.BuildUserModel();
            var user = md.GetById(userId, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("To change password you need a email");
            }
            if (hasPassword)
            {
                var mdUser = md.LoginWithEmail(user.Email, oldPassword, _userFactory);
                if (mdUser == null)
                {
                    throw new Exception("Email or password is wrong");
                }
            }
            _logger.LogTrace("Changing password using old password: email: {0}, old password: {1}, new password: {2}", user.Email, oldPassword, newPassword);
            md.ChangePassword(user.UserId, newPassword, _userFactory);
            _logger.LogTrace("Password successful changed using old password");
        }

        public async Task<bool> SendRecoveryEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email cant be empty");
            }
            var md = _userFactory.BuildUserModel();
            var user = md.GetByEmail(email, _userFactory);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var recoveryHash = md.GenerateRecoveryHash(user.UserId, _userFactory);
            var recoveryUrl = $"https://nochainswap.org/recoverypassword/{recoveryHash}";

            var textMessage =
                $"Hi {user.Name},\r\n\r\n" +
                "We received a request to reset your password. If you made this request, " +
                "please click the link below to reset your password:\r\n\r\n" +
                recoveryUrl + "\r\n\r\n" +
                "If you didn’t request a password reset, please ignore this email or contact " +
                "our support team if you have any concerns.\r\n\r\n" +
                "Best regards,\r\n" +
                "NoChainSwap Team";
            var htmlMessage =
                $"Hi <b>{user.Name}</b>,<br />\r\n<br />\r\n" +
                "We received a request to reset your password. If you made this request, " +
                "please click the link below to reset your password:<br />\r\n<br />\r\n" +
                $"<a href=\"{recoveryUrl}\">{recoveryUrl}</a><br />\r\n<br />\r\n" +
                "If you didn’t request a password reset, please ignore this email or contact " +
                "our support team if you have any concerns.<br />\r\n<br />\r\n" +
                "Best regards,<br />\r\n" +
                "<b>NoChainSwap Team</b>";

            var mail = new MailerInfo
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
            await _mailClient.SendmailAsync(mail);
            return await Task.FromResult(true);
        }

        private async Task<string> GenerateSlug(IUserModel md)
        {
            string newSlug;
            int c = 0;
            do
            {
                newSlug = await _stringClient.GenerateSlugAsync(!string.IsNullOrEmpty(md.Slug) ? md.Slug : md.Name);
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
                    var modelPhone = _phoneFactory.BuildUserPhoneModel();
                    modelPhone.UserId = user.UserId;
                    modelPhone.Phone = phone.Phone;
                    modelPhone.Insert(_phoneFactory);
                }
            }
        }

        private void InsertAddresses(UserInfo user)
        {
            if (user.Addresses != null && user.Addresses.Count() > 0)
            {
                foreach (var addr in user.Addresses)
                {
                    var modelAddr = _addrFactory.BuildUserAddressModel();
                    modelAddr.UserId = user.UserId;
                    modelAddr.ZipCode = addr.ZipCode;
                    modelAddr.Address = addr.Address;
                    modelAddr.Complement = addr.Complement;
                    modelAddr.Neighborhood = addr.Neighborhood;
                    modelAddr.City = addr.City;
                    modelAddr.State = addr.State;
                    modelAddr.Insert(_addrFactory);
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
                    throw new Exception($"Phone is empty");
                }
                else
                {
                    phone.Phone = await _stringClient.OnlyNumbersAsync(phone.Phone.Trim());
                    if (string.IsNullOrEmpty(phone.Phone))
                    {
                        throw new Exception($"{phone.Phone} is not a valid phone");
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
                if (string.IsNullOrEmpty(addr.ZipCode))
                {
                    throw new Exception($"ZipCode is empty");
                }
                else
                {
                    addr.ZipCode = await _stringClient.OnlyNumbersAsync(addr.ZipCode);
                    if (string.IsNullOrEmpty(addr.ZipCode))
                    {
                        throw new Exception($"{addr.ZipCode} is not a valid zip code");
                    }
                }
                if (string.IsNullOrEmpty(addr.Address))
                {
                    throw new Exception("Address is empty");
                }
                if (string.IsNullOrEmpty(addr.Complement))
                {
                    throw new Exception("Address is empty");
                }
                if (string.IsNullOrEmpty(addr.Neighborhood))
                {
                    throw new Exception("Neighborhood is empty");
                }
                if (string.IsNullOrEmpty(addr.City))
                {
                    throw new Exception("City is empty");
                }
                if (string.IsNullOrEmpty(addr.State))
                {
                    throw new Exception("State is empty");
                }
            }
        }

        public async Task<IUserModel> Insert(UserInfo user)
        {
            var model = _userFactory.BuildUserModel();
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new Exception("Name is empty");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Email is empty");
            }
            else
            {
                if (!await _mailClient.IsValidEmailAsync(user.Email))
                {
                    throw new Exception("Email is not valid");
                }
                var userWithEmail = model.GetByEmail(user.Email, _userFactory);
                if (userWithEmail != null)
                {
                    throw new Exception("User with email already registered");
                }
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new Exception("Password is empty");
            }
            if (!string.IsNullOrEmpty(user.IdDocument))
            {
                user.IdDocument = await _stringClient.OnlyNumbersAsync(user.IdDocument);
                if (!await _documentClient.validarCpfOuCnpjAsync(user.IdDocument))
                {
                    throw new Exception($"{user.IdDocument} is not a valid CPF or CNPJ");
                }
            }
            await ValidatePhones(user);
            await ValidateAddresses(user);

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

            var md = model.Insert(_userFactory);

            user.UserId = md.UserId;
            InsertPhones(user);
            InsertAddresses(user);

            md.ChangePassword(user.UserId, user.Password, _userFactory);

            return md;
        }

        public async Task<IUserModel> Update(UserInfo user)
        {
            IUserModel model = null;
            if (!(user.UserId > 0))
            {
                throw new Exception("User not found");
            }
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new Exception("Name is empty");
            }
            model = _userFactory.BuildUserModel().GetById(user.UserId, _userFactory);
            if (model == null)
            {
                throw new Exception("User not exists");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Email is empty");
            }
            else
            {
                if (!await _mailClient.IsValidEmailAsync(user.Email))
                {
                    throw new Exception("Email is not valid");
                }
                var userWithEmail = model.GetByEmail(user.Email, _userFactory);
                if (userWithEmail != null && userWithEmail.UserId != model.UserId)
                {
                    throw new Exception("User with email already registered");
                }
            }
            if (!string.IsNullOrEmpty(user.IdDocument))
            {
                user.IdDocument = await _stringClient.OnlyNumbersAsync(user.IdDocument);
                if (!await _documentClient.validarCpfOuCnpjAsync(user.IdDocument))
                {
                    throw new Exception($"{user.IdDocument} is not a valid CPF or CNPJ");
                }
            }
            await ValidatePhones(user);
            await ValidateAddresses(user);

            model.Slug = user.Slug;
            model.Name = user.Name;
            model.Email = user.Email;
            model.BirthDate = user.BirthDate;
            model.IdDocument = user.IdDocument;
            model.PixKey = user.PixKey;
            model.UpdatedAt = DateTime.Now;
            model.Slug = await GenerateSlug(model);

            model.Update(_userFactory);

            var modelPhone = _phoneFactory.BuildUserPhoneModel();
            modelPhone.DeleteAllByUser(model.UserId);
            InsertPhones(user);

            var modelAddr = _addrFactory.BuildUserAddressModel();
            modelAddr.DeleteAllByUser(model.UserId);
            InsertAddresses(user);

            return model;
        }

        public IUserModel GetUserByEmail(string email)
        {
            return _userFactory.BuildUserModel().GetByEmail(email, _userFactory);
        }

        public IUserModel GetUserByID(long userId)
        {
            return _userFactory.BuildUserModel().GetById(userId, _userFactory);
        }

        public IUserModel GetUserByToken(string token)
        {
            var tokenModel = _tokenFactory.BuildUserTokenModel().GetByToken(token, _tokenFactory);
            if (tokenModel == null)
            {
                return null;
            }
            return _userFactory.BuildUserModel().GetById(tokenModel.UserId, _userFactory);
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
                ImageUrl = await _fileClient.GetFileUrlAsync(GetBucketName(), md.Image),
                Name = md.Name,
                Email = md.Email,
                IdDocument = md.IdDocument,
                PixKey = md.PixKey,
                BirthDate = md.BirthDate,
                CreatedAt = md.CreatedAt,
                UpdatedAt = md.UpdatedAt,
                IsAdmin = md.IsAdmin,
                Phones = _phoneFactory.BuildUserPhoneModel()
                    .ListByUser(md.UserId, _phoneFactory)
                    .Select(x => new UserPhoneInfo
                    {
                        Phone = x.Phone
                    }).ToList(),
                Addresses = _addrFactory.BuildUserAddressModel()
                    .ListByUser(md.UserId, _addrFactory)
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
            return _userFactory.BuildUserModel().GetByStripeId(stripeId, _userFactory);
        }

        public IUserModel GetBySlug(string slug)
        {
            return _userFactory.BuildUserModel().GetBySlug(slug, _userFactory);
        }

        public IList<IUserModel> ListUsers(int take)
        {
            return _userFactory.BuildUserModel().ListUsers(take, _userFactory).ToList();
        }
    }
}
