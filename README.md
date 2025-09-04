# NAuth

**NAuth** is a complete, modular authentication framework designed for fast and secure user management in modern web applications. Built using **.NET Core**, **React**, **Bootstrap**, and **PostgreSQL**, NAuth provides a robust solution for user registration, login, password recovery, and profile updates — ready to be integrated into any full-stack project.

---

## 🚀 Features

- 🔐 User registration with email confirmation  
- 🔑 Login with Token authentication  
- 🔄 Password reset via email (secure token-based flow)  
- ✏️ Profile update and password change  
- 🧰 Ready-to-use frontend components with Bootstrap  
- 🗄️ PostgreSQL schema and migrations included  
- 📦 Modular architecture for reuse across multiple projects  
- 🌐 REST API with Swagger documentation

---

## 🛠️ Technologies Used

| Layer      | Technology        |
|------------|-------------------|
| Backend    | .NET Core (ASP.NET Web API) |
| Frontend   | React + Bootstrap |
| Database   | PostgreSQL        |
| Auth       | JWT + Email Tokens |
| Dev Tools  | Docker, Swagger, EF Core |

---

## 📁 Project Structure

```
NAuth/
├── Backend/                 # .NET Core Web API
│   └── NAuth/
│       ├── DB.Infra         # Repository
│       ├── NAuth.API        # API Rest
│       ├── NAuth.Client     # NAuth Client
│       ├── NAuth.Domain     # NAuth Domain
│       │   ├── Interfaces
│       │   │   ├── Core
│       │   │   ├── Factory
│       │   │   ├── Models
│       │   │   └── Services
│       │   └── Impl
│       │       ├── Core
│       │       ├── Factory
│       │       ├── Models
│       │       └── Services
│       ├── NAuth.DTO
│       └── NAuth.Test       # Unit Test Layer
├── Frontend/                # React + Bootstrap frontend
│   ├── nauth-core/          # React Hook           
│   └── nauth-app/           # React App
├── SQL/                     # Database Structure
└── README.md
```

---

## ⚙️ How to Run

### 1. Clone the repository

```bash
git clone https://github.com/landim32/nauth.git
cd nauth
```

### 2. Configure environment variables

Ao alterar o Frontend/nauth-core é necessário atualizar a biblioteca hook no Frontend/nauth-app com o comando abaixo:

```bash
npm install --legacy-peer-deps ../nauth-core
``` 

Edit `.env` files in both `backend` and `frontend` folders to set your PostgreSQL connection, email settings, and frontend URL.

### 3. Start with Docker (recommended)

```bash
docker-compose up --build
```

The app will be available at:
- Frontend: `http://localhost:3000`
- Backend API: `http://localhost:44374`

---

## 🧪 API Documentation

Once running, access the Swagger UI at:  
`http://localhost:44374/swagger/index.html`

---

## 📦 Integration

You can integrate NAuth with your existing projects by:

1. Importing backend services as a module or via NuGet (coming soon).
2. Reusing frontend components (`LoginForm`, `ResetPasswordForm`, `UserProfile`) directly or as a standalone package.
3. Connecting via API from any frontend using standard HTTP requests and JWT.

---

## 📧 Email Support

NAuth includes ready-to-use email templates and SMTP support. Configure your provider via environment variables.

---

## 🔒 Security

- All sensitive tokens are time-bound and hashed
- Passwords are securely stored using strong hashing algorithms (PBKDF2 or BCrypt)
- Follows best practices for authentication flows and CSRF protection

---

## 🧩 Future Plans

- Two-factor authentication (2FA)
- OAuth2 and social logins (Google, GitHub, Facebook)
- Admin dashboard for user management
- Role-based access control (RBAC)

---

## 👨‍💻 Author

Developed by [Rodrigo Landim Carneiro](https://github.com/landim32)

---

## 📄 License

MIT License. Feel free to use and contribute.
