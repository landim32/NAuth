using NAuth.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.Domain.Interfaces.Factory
{
    public interface IUserPhoneDomainFactory
    {
        IUserPhoneModel BuildUserPhoneModel();
    }
}
