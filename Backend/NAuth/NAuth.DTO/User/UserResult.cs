using System;
using NAuth.DTO.Domain;

namespace NAuth.DTO.User
{
    public class UserResult : StatusResult
    {
        public UserInfo User { get; set; }
    }
}
