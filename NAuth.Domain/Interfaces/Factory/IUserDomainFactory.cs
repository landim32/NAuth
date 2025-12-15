using System;
using NAuth.Domain.Interfaces.Models;

namespace NAuth.Domain.Interfaces.Factory
{
    public interface IUserDomainFactory
    {
        IUserModel BuildUserModel();
    }
}
