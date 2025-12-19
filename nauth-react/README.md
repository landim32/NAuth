# @nauth/react-auth

Modern React authentication component library for NAuth API integration. Built with TypeScript, Tailwind CSS, and designed as a distributable NPM package.

[![npm version](https://badge.fury.io/js/%40nauth%2Freact-auth.svg)](https://www.npmjs.com/package/@nauth/react-auth)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

✨ **Complete Auth Suite** - Login, Register, Password Recovery, Change Password  
🎨 **Theme Support** - Light/Dark mode with system detection  
📦 **Tree-shakeable** - Import only what you need  
🔒 **Type-Safe** - Full TypeScript support  
🎯 **Security** - Device fingerprinting with FingerprintJS  
♿ **Accessible** - WCAG compliant  
📱 **Responsive** - Mobile-first design  

## Installation

```bash
npm install @nauth/react-auth react-router-dom

# If you don't have Tailwind CSS
npm install -D tailwindcss postcss autoprefixer tailwindcss-animate
npx tailwindcss init -p
```

### Configure Tailwind

```javascript
// tailwind.config.js
export default {
  darkMode: ['class'],
  content: [
    './src/**/*.{js,ts,jsx,tsx}',
    './node_modules/@nauth/react-auth/dist/**/*.{js,ts,jsx,tsx}',
  ],
  plugins: [require('tailwindcss-animate')],
};
```

```css
/* src/index.css */
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## Quick Start

### 1. Setup Providers

```tsx
import { BrowserRouter } from 'react-router-dom';
import { NAuthProvider, ThemeProvider } from '@nauth/react-auth';
import '@nauth/react-auth/styles';
import './index.css';

function App() {
  return (
    <BrowserRouter>
      <ThemeProvider defaultTheme="system">
        <NAuthProvider
          config={{
            apiUrl: import.meta.env.VITE_API_URL,
            enableFingerprinting: true,
          }}
        >
          <YourApp />
        </NAuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
}
```

```env
# .env
VITE_API_URL=https://your-nauth-api.com
```

### 2. Use Components

```tsx
import { LoginForm, RegisterForm, useAuth, useProtectedRoute } from '@nauth/react-auth';
import { useNavigate } from 'react-router-dom';

// Login Page
function LoginPage() {
  const navigate = useNavigate();
  return (
    <div className="min-h-screen flex items-center justify-center">
      <LoginForm
        onSuccess={() => navigate('/dashboard')}
        showRememberMe
        showForgotPassword
      />
    </div>
  );
}

// Protected Dashboard
function Dashboard() {
  const { user, logout } = useAuth();
  useProtectedRoute({ redirectTo: '/login' });

  return (
    <div>
      <h1>Welcome, {user?.name}</h1>
      <button onClick={logout}>Logout</button>
    </div>
  );
}
```

## Components

**Authentication Forms:**
- `LoginForm` - Email/password login with validation
- `RegisterForm` - Multi-step registration with password strength
- `ForgotPasswordForm` - Password recovery request
- `ResetPasswordForm` - Password reset with token
- `ChangePasswordForm` - Change password for authenticated users

**UI Components:** `Button`, `Input`, `Label`, `Card`, `Avatar`, `DropdownMenu`, `Toaster`

## Hooks

```tsx
// Authentication state
const { user, isAuthenticated, login, logout, isLoading } = useAuth();

// User management
const { user, updateUser, changePassword, uploadImage } = useUser();

// Route protection
useProtectedRoute({ redirectTo: '/login', requireAdmin: false });

// Theme management
const { theme, setTheme } = useTheme();
```

## Configuration

```tsx
<NAuthProvider
  config={{
    apiUrl: 'https://your-api.com',           // Required
    timeout: 30000,                            // Optional
    enableFingerprinting: true,                // Optional
    storageType: 'localStorage',               // Optional
    redirectOnUnauthorized: '/login',          // Optional
    onAuthChange: (user) => {},                // Optional
    onLogin: (user) => {},                     // Optional
    onLogout: () => {},                        // Optional
  }}
>
  <App />
</NAuthProvider>
```

## API Client

```tsx
import { createNAuthClient } from '@nauth/react-auth';

const api = createNAuthClient({ apiUrl: 'https://your-api.com' });

await api.login({ email, password });
await api.getMe();
await api.updateUser({ name: 'New Name' });
await api.uploadImage(file);
```

## Customization

```tsx
<LoginForm
  className="shadow-2xl"
  styles={{
    container: 'bg-white',
    button: 'bg-purple-600',
  }}
/>
```

## Utilities

```tsx
import {
  validateCPF,
  validateCNPJ,
  validateEmail,
  formatPhone,
  validatePasswordStrength,
} from '@nauth/react-auth';
```

## TypeScript

```tsx
import type {
  UserInfo,
  LoginCredentials,
  NAuthConfig,
  Theme,
} from '@nauth/react-auth';
```

## Project Structure

```
src/
├── components/       # Auth forms + UI components
├── contexts/         # NAuthContext, ThemeContext
├── hooks/            # useAuth, useUser, useProtectedRoute, useTheme
├── services/         # NAuth API client
├── types/            # TypeScript definitions
├── utils/            # Validators, formatters
└── styles/           # Tailwind CSS
```

## Development

```bash
npm install        # Install dependencies
npm run dev        # Development mode
npm run build      # Build library
npm test           # Run tests
npm run storybook  # Component documentation
```

## Publishing

```bash
npm run build
npm publish --access public
```

## Changelog

### [1.0.0] - 2024-12-18
- Initial release
- Complete authentication suite
- Theme support
- TypeScript support
- Brazilian document validation
- Comprehensive documentation

## License

MIT © [Rodrigo Landim](https://github.com/landim32)

## Links

- [GitHub](https://github.com/landim32/NAuth)
- [NPM](https://www.npmjs.com/package/@nauth/react-auth)
