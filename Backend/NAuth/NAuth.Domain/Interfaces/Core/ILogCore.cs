using System;
using NAuth.Domain.Impl.Core;
using Microsoft.Extensions.Logging;

namespace NAuth.Domain.Interfaces.Core
{
    public interface ILogCore
    {
        void Log(string message, Levels level);
    }
}
