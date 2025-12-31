# NAuth.ACL

[![NuGet](https://img.shields.io/nuget/v/NAuth.ACL.svg)](https://www.nuget.org/packages/NAuth.ACL/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NAuth.ACL.svg)](https://www.nuget.org/packages/NAuth.ACL/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Access Control Layer (ACL) client library for NAuth authentication system. Provides HTTP client interfaces and implementations for user and role management, authentication handlers, and JWT token processing for consuming NAuth API services.

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package NAuth.ACL
```

Or via Package Manager Console:

```powershell
Install-Package NAuth.ACL
```

## ✨ Features

- **HTTP Client**: Ready-to-use HTTP clients for NAuth API
- **User Management**: Complete user CRUD operations
- **Role Management**: Role and permission handling
- **Authentication**: Login, logout, and token management
- **Password Management**: Change and reset password operations
- **File Upload**: Image upload for user profiles
- **JWT Handling**: Built-in JWT token processing
- **ASP.NET Core Integration**: Authentication handler for middleware
- **.NET 8 Compatible**: Built for modern .NET applications

## 🚀 Quick Start

### 1. Configure Services

Add NAuth.ACL to your ASP.NET Core application:

```csharp
using NAuth.ACL;
using NAuth.ACL.Interfaces;
using NAuth.DTO.Settings;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure NAuth settings
        services.Configure<NAuthSetting>(options =>
        {
            options.ApiUrl = "https://your-nauth-api.com/api";
            options.JwtSecret = "your-jwt-secret-key";
            options.BucketName = "user-images";
        });

        // Add HttpClient
        services.AddHttpClient();

        // Register NAuth clients
        services.AddScoped<IUserClient, UserClient>();
        services.AddScoped<IRoleClient, RoleClient>();

        // Add authentication with NAuth handler
        services.AddAuthentication("NAuth")
            .AddScheme<NAuthHandlerOptions, NAuthHandler>("NAuth", options => { });
    }
}
```

### 2. Configure appsettings.json

```json
{
  "NAuthSetting": {
    "ApiUrl": "https://your-nauth-api.com/api",
    "JwtSecret": "your-super-secret-jwt-key-min-32-characters",
    "BucketName": "nauth-user-images"
  }
}
```

## 👤 User Client

### IUserClient Interface

Complete user management operations:

```csharp
public interface IUserClient
{
    // Session Management
    UserInfo? GetUserInSession(HttpContext httpContext);
    
    // User Retrieval
    Task<UserInfo?> GetMeAsync(string token);
    Task<UserInfo?> GetByIdAsync(long userId, string token);
    Task<UserInfo?> GetByTokenAsync(string token);
    Task<UserInfo?> GetByEmailAsync(string email);
    Task<UserInfo?> GetBySlugAsync(string slug);
    Task<IList<UserInfo>> ListAsync(int take);
    
    // User Management
    Task<UserInfo?> InsertAsync(UserInsertedInfo user);
    Task<UserInfo?> UpdateAsync(UserInfo user, string token);
    
    // Authentication
    Task<UserTokenResult?> LoginWithEmailAsync(LoginParam param);
    
    // Password Management
    Task<bool> HasPasswordAsync(string token);
    Task<bool> ChangePasswordAsync(ChangePasswordParam param, string token);
    Task<bool> SendRecoveryMailAsync(string email);
    Task<bool> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param);
    
    // File Upload
    Task<string> UploadImageUserAsync(Stream fileStream, string fileName, string token);
}
```

### Usage Examples

#### User Registration

```csharp
using NAuth.ACL.Interfaces;
using NAuth.DTO.User;

public class UserService
{
    private readonly IUserClient _userClient;

    public UserService(IUserClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<UserInfo> RegisterUserAsync()
    {
        var newUser = new UserInsertedInfo
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "SecureP@ssw0rd",
            BirthDate = new DateTime(1990, 1, 1),
            IdDocument = "12345678901",
            Roles = new List<RoleInfo>
            {
                new RoleInfo { RoleId = 1 }
            }
        };

        var result = await _userClient.InsertAsync(newUser);
        return result;
    }
}
```

#### User Login

```csharp
public async Task<UserTokenResult> LoginAsync(string email, string password)
{
    var loginParam = new LoginParam
    {
        Email = email,
        Password = password
    };

    var result = await _userClient.LoginWithEmailAsync(loginParam);
    
    if (result != null)
    {
        // Store token for subsequent requests
        var token = result.Token;
        var user = result.User;
        
        Console.WriteLine($"Logged in as: {user.Name}");
        Console.WriteLine($"Token: {token}");
    }
    
    return result;
}
```

#### Get Current User

```csharp
public async Task<UserInfo> GetCurrentUserAsync(string token)
{
    var user = await _userClient.GetMeAsync(token);
    
    Console.WriteLine($"User ID: {user.UserId}");
    Console.WriteLine($"Name: {user.Name}");
    Console.WriteLine($"Email: {user.Email}");
    Console.WriteLine($"Is Admin: {user.IsAdmin}");
    
    return user;
}
```

#### Update User Profile

```csharp
public async Task<UserInfo> UpdateProfileAsync(UserInfo user, string token)
{
    user.Name = "Jane Doe";
    user.Email = "jane.doe@example.com";
    
    var updatedUser = await _userClient.UpdateAsync(user, token);
    return updatedUser;
}
```

#### Password Management

```csharp
// Change password (authenticated user)
public async Task<bool> ChangePasswordAsync(string token)
{
    var changeParam = new ChangePasswordParam
    {
        OldPassword = "OldPassword123",
        NewPassword = "NewSecureP@ssw0rd"
    };

    var success = await _userClient.ChangePasswordAsync(changeParam, token);
    return success;
}

// Send recovery email
public async Task<bool> SendRecoveryEmailAsync(string email)
{
    var success = await _userClient.SendRecoveryMailAsync(email);
    return success;
}

// Reset password using hash from email
public async Task<bool> ResetPasswordAsync(string recoveryHash, string newPassword)
{
    var resetParam = new ChangePasswordUsingHashParam
    {
        RecoveryHash = recoveryHash,
        NewPassword = newPassword
    };

    var success = await _userClient.ChangePasswordUsingHashAsync(resetParam);
    return success;
}
```

#### Upload User Image

```csharp
public async Task<string> UploadUserImageAsync(IFormFile file, string token)
{
    using var stream = file.OpenReadStream();
    var imageUrl = await _userClient.UploadImageUserAsync(
        stream, 
        file.FileName, 
        token
    );
    
    Console.WriteLine($"Image uploaded: {imageUrl}");
    return imageUrl;
}
```

#### Get User from Session

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IUserClient _userClient;

    public ProfileController(IUserClient userClient)
    {
        _userClient = userClient;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetProfile()
    {
        var user = _userClient.GetUserInSession(HttpContext);
        
        if (user == null)
        {
            return Unauthorized();
        }
        
        return Ok(user);
    }
}
```

## 👥 Role Client

### IRoleClient Interface

Role and permission management:

```csharp
public interface IRoleClient
{
    Task<IList<RoleInfo>> ListAsync();
    Task<RoleInfo?> GetByIdAsync(long roleId);
    Task<RoleInfo?> GetBySlugAsync(string slug);
    Task<RoleInfo?> InsertAsync(RoleInfo role);
    Task<RoleInfo?> UpdateAsync(RoleInfo role);
    Task<bool> DeleteAsync(long roleId);
}
```

### Usage Examples

```csharp
using NAuth.ACL.Interfaces;
using NAuth.DTO.User;

public class RoleService
{
    private readonly IRoleClient _roleClient;

    public RoleService(IRoleClient roleClient)
    {
        _roleClient = roleClient;
    }

    // List all roles
    public async Task<IList<RoleInfo>> GetAllRolesAsync()
    {
        var roles = await _roleClient.ListAsync();
        return roles;
    }

    // Create new role
    public async Task<RoleInfo> CreateRoleAsync()
    {
        var newRole = new RoleInfo
        {
            Slug = "moderator",
            Name = "Moderator"
        };

        var result = await _roleClient.InsertAsync(newRole);
        return result;
    }

    // Update role
    public async Task<RoleInfo> UpdateRoleAsync(long roleId)
    {
        var role = await _roleClient.GetByIdAsync(roleId);
        role.Name = "Super Moderator";

        var updated = await _roleClient.UpdateAsync(role);
        return updated;
    }

    // Delete role
    public async Task<bool> DeleteRoleAsync(long roleId)
    {
        var success = await _roleClient.DeleteAsync(roleId);
        return success;
    }
}
```

## 🔐 Authentication Handler

### NAuthHandler

Custom authentication handler for ASP.NET Core middleware that validates JWT tokens from NAuth:

```csharp
public class NAuthHandler : AuthenticationHandler<NAuthHandlerOptions>
{
    // Automatically validates JWT tokens
    // Extracts user claims from token
    // Integrates with ASP.NET Core authentication
}
```

### Usage in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    [HttpGet]
    [Authorize] // Uses NAuthHandler automatically
    public IActionResult GetSecureData()
    {
        var userId = User.FindFirst("userId")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var isAdmin = User.FindFirst("isAdmin")?.Value;

        return Ok(new
        {
            UserId = userId,
            Email = email,
            IsAdmin = isAdmin
        });
    }

    [HttpGet("admin")]
    [Authorize(Roles = "admin")] // Require admin role
    public IActionResult GetAdminData()
    {
        return Ok("Admin only data");
    }
}
```

## ⚙️ Advanced Configuration

### Custom HTTP Client Configuration

```csharp
services.AddHttpClient<IUserClient, UserClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

### Retry Policy with Polly

```csharp
using Polly;
using Polly.Extensions.Http;

services.AddHttpClient<IUserClient, UserClient>()
    .AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

## ⚠️ Error Handling

All client methods may throw exceptions. Implement proper error handling:

```csharp
try
{
    var user = await _userClient.GetMeAsync(token);
}
catch (HttpRequestException ex)
{
    // Handle network errors
    _logger.LogError(ex, "Network error occurred");
}
catch (Exception ex)
{
    // Handle other errors
    _logger.LogError(ex, "Error retrieving user");
}
```

## 📦 Dependencies

- **Microsoft.AspNetCore.Authentication** (2.3.0)
- **System.IdentityModel.Tokens.Jwt** (8.15.0)
- **NAuth.DTO** (latest)

## 🔗 Related Packages

- **NAuth.DTO**: Data Transfer Objects library
- **NAuth.API**: REST API implementation
- **NAuth.Domain**: Business logic layer

## 💡 Best Practices

1. **Token Storage**: Store JWT tokens securely (HttpOnly cookies or secure storage)
2. **Token Refresh**: Implement token refresh before expiration
3. **Error Handling**: Always wrap API calls in try-catch blocks
4. **Logging**: Use structured logging for debugging
5. **Dependency Injection**: Always use DI for client instances
6. **Configuration**: Use strongly-typed configuration with `IOptions<NAuthSetting>`

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://opensource.org/licenses/MIT) file for details.

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/NAuth.ACL/)
- [GitHub Repository](https://github.com/landim32/NAuth/tree/main/NAuth.ACL)
- [Documentation](https://github.com/landim32/NAuth/wiki)
- [API Documentation](https://github.com/landim32/NAuth/wiki/API)

## 💬 Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/landim32/NAuth).

---

Made with ❤️ by [Rodrigo Landim](https://github.com/landim32) at Emagine
