using NAuth.Domain.Impl.Models;
using NAuth.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.Domain.Interfaces.Services
{
    public interface IImageService
    {
        string GetImageUrl(string fileName);
        string InsertFromStream(Stream stream, string name);
        string InsertToUser(Stream stream, long userId);
    }
}
