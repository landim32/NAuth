# NAuth.DTO

[![NuGet](https://img.shields.io/nuget/v/NAuth.DTO.svg)](https://www.nuget.org/packages/NAuth.DTO/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NAuth.DTO.svg)](https://www.nuget.org/packages/NAuth.DTO/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Data Transfer Objects (DTOs) library for NAuth authentication and authorization system. Contains all data models, settings, and converters used across NAuth components for user management, roles, and authentication.

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package NAuth.DTO
```

Or via Package Manager Console:

```powershell
Install-Package NAuth.DTO
```

## ✨ Features

- **User DTOs**: Complete user data models for authentication and management
- **Role DTOs**: Role and permission data structures
- **Authentication Models**: Login parameters, token results, and password management
- **Settings**: Configuration objects for NAuth services
- **Converters**: Custom JSON converters for proper serialization
- **Validation**: Built-in validation attributes and structures
- **.NET 8 Compatible**: Built for modern .NET applications

## 📚 Core Components

### User Models

#### UserInfo
Complete user information including profile, roles, addresses, and phones.

```csharp
public class UserInfo
{
    public long UserId { get; set; }
    public string Slug { get; set; }
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Hash { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime? BirthDate { get; set; }
    public string IdDocument { get; set; }
    public string PixKey { get; set; }
    public string Password { get; set; }
    public int Status { get; set; }
    public IList<RoleInfo> Roles { get; set; }
    public IList<UserPhoneInfo> Phones { get; set; }
    public IList<UserAddressInfo> Addresses { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### UserInsertedInfo
Simplified model for user registration (without system-generated fields).

```csharp
public class UserInsertedInfo
{
    public string Slug { get; set; }
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime? BirthDate { get; set; }
    public string IdDocument { get; set; }
    public string PixKey { get; set; }
    public string Password { get; set; }
    public IList<RoleInfo> Roles { get; set; }
    public IList<UserPhoneInfo> Phones { get; set; }
    public IList<UserAddressInfo> Addresses { get; set; }
}
```

#### LoginParam
Authentication credentials.

```csharp
public class LoginParam
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

#### UserTokenResult
Authentication result with user info and JWT token.

```csharp
public class UserTokenResult
{
    public string Token { get; set; }
    public UserInfo User { get; set; }
}
```

### Password Management

#### ChangePasswordParam
Change password with current password verification.

```csharp
public class ChangePasswordParam
{
    [JsonPropertyName("oldPassword")]
    public string OldPassword { get; set; }
    
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; }
}
```

#### ChangePasswordUsingHashParam
Reset password using recovery hash.

```csharp
public class ChangePasswordUsingHashParam
{
    [JsonPropertyName("recoveryHash")]
    public string RecoveryHash { get; set; }
    
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; }
}
```

### Role Models

#### RoleInfo
Role information with permissions.

```csharp
public class RoleInfo
{
    public long RoleId { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
}
```

### Search and Pagination

#### UserSearchParam
Parameters for user search.

```csharp
public class UserSearchParam
{
    public string SearchTerm { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

#### PagedResult<T>
Generic paged result container.

```csharp
public class PagedResult<T>
{
    public IList<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}
```

### Settings

#### NAuthSetting
Configuration settings for NAuth services.

```csharp
public class NAuthSetting
{
    public string ApiUrl { get; set; }
    public string JwtSecret { get; set; }
    public string BucketName { get; set; }
}
```

## 💻 Usage Examples

### Basic User Model Usage

```csharp
using NAuth.DTO.User;

// Create a new user for registration
var newUser = new UserInsertedInfo
{
    Name = "John Doe",
    Email = "john.doe@example.com",
    Password = "SecureP@ssw0rd",
    BirthDate = new DateTime(1990, 1, 1),
    IdDocument = "12345678901",
    Roles = new List<RoleInfo>
    {
        new RoleInfo { RoleId = 1, Slug = "user", Name = "User" }
    },
    Phones = new List<UserPhoneInfo>
    {
        new UserPhoneInfo { Phone = "5511999999999" }
    },
    Addresses = new List<UserAddressInfo>
    {
        new UserAddressInfo
        {
            ZipCode = "01234567",
            Address = "123 Main St",
            Complement = "Apt 101",
            Neighborhood = "Downtown",
            City = "São Paulo",
            State = "SP"
        }
    }
};
```

### Login Example

```csharp
using NAuth.DTO.User;

// Prepare login credentials
var loginParam = new LoginParam
{
    Email = "john.doe@example.com",
    Password = "SecureP@ssw0rd"
};

// After successful authentication, you receive:
var tokenResult = new UserTokenResult
{
    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    User = userInfo
};
```

### Password Management

```csharp
using NAuth.DTO.User;

// Change password (authenticated user)
var changePassword = new ChangePasswordParam
{
    OldPassword = "OldP@ssw0rd",
    NewPassword = "NewP@ssw0rd123"
};

// Reset password using recovery hash
var resetPassword = new ChangePasswordUsingHashParam
{
    RecoveryHash = "abc123def456",
    NewPassword = "NewP@ssw0rd123"
};
```

### Pagination Example

```csharp
using NAuth.DTO.User;

// Search users with pagination
var searchParam = new UserSearchParam
{
    SearchTerm = "john",
    Page = 1,
    PageSize = 20
};

// Results come in a paged format
var results = new PagedResult<UserInfo>
{
    Items = userList,
    Page = 1,
    PageSize = 20,
    TotalCount = 150
};

Console.WriteLine($"Showing page {results.Page} of {results.TotalPages}");
Console.WriteLine($"Has next page: {results.HasNextPage}");
```

## 🔐 JSON Serialization

All DTOs are properly decorated with JSON attributes for seamless serialization with both `Newtonsoft.Json` and `System.Text.Json`:

```csharp
[JsonPropertyName("userId")]
public long UserId { get; set; }

[JsonPropertyName("email")]
public string Email { get; set; }
```

### Custom Converters

The library includes custom converters for special cases:

#### NullableDateTimeConverter
Handles nullable DateTime serialization properly.

```csharp
[JsonConverter(typeof(NullableDateTimeConverter))]
public DateTime? BirthDate { get; set; }
```

## 📊 User Status Enum

User status values for account state management:

```csharp
public enum UserStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Blocked = 4
}
```

## 🔗 Related Packages

- **NAuth.ACL**: Access Control Layer client library for consuming NAuth services
- **NAuth.API**: REST API implementation for authentication services
- **NAuth.Domain**: Business logic and domain models

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://opensource.org/licenses/MIT) file for details.

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/NAuth.DTO/)
- [GitHub Repository](https://github.com/landim32/NAuth/tree/main/NAuth.DTO)
- [Documentation](https://github.com/landim32/NAuth/wiki)

## 💬 Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/landim32/NAuth).

---

Made with ❤️ by [Rodrigo Landim](https://github.com/landim32) at Emagine
