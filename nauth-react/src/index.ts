// Main entry point for @nauth/react-auth package

// Styles - Import first
import './styles/index.css';

// Authentication Components
export { LoginForm } from './components/LoginForm';
export { RegisterForm } from './components/RegisterForm';
export { ForgotPasswordForm } from './components/ForgotPasswordForm';
export { ResetPasswordForm } from './components/ResetPasswordForm';
export { ChangePasswordForm } from './components/ChangePasswordForm';

// Context Providers & Hooks
export { NAuthProvider, useNAuth } from './contexts/NAuthContext';
export type { NAuthConfig, NAuthContextValue, NAuthProviderProps } from './contexts/NAuthContext';

export { ThemeProvider, useTheme } from './contexts/ThemeContext';
export type { ThemeProviderProps } from './contexts/ThemeContext';

// Custom Hooks
export { useAuth } from './hooks/useAuth';
export { useUser } from './hooks/useUser';
export { useProtectedRoute } from './hooks/useProtectedRoute';
export type { UseProtectedRouteOptions } from './hooks/useProtectedRoute';

// API Client
export { NAuthAPI, createNAuthClient } from './services/nauth-api';

// UI Components
export { Button } from './components/ui/button';
export { Input } from './components/ui/input';
export { Label } from './components/ui/label';
export { Card, CardHeader, CardFooter, CardTitle, CardDescription, CardContent } from './components/ui/card';
export { Avatar, AvatarImage, AvatarFallback } from './components/ui/avatar';
export {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuCheckboxItem,
  DropdownMenuRadioItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuShortcut,
  DropdownMenuGroup,
  DropdownMenuPortal,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuRadioGroup,
} from './components/ui/dropdown-menu';
export { Toaster } from './components/ui/toaster';

// TypeScript Types
export type {
  UserInfo,
  AuthSession,
  UserAddress,
  UserPhone,
  UserRole,
  LoginCredentials,
  RegisterData,
  ChangePasswordData,
  ResetPasswordData,
  ApiResponse,
  ApiError,
  Theme,
  ThemeContextValue,
  LoginFormProps,
  RegisterFormProps,
  ForgotPasswordFormProps,
  ResetPasswordFormProps,
  ChangePasswordFormProps,
} from './types';

// Utility Functions
export {
  validateCPF,
  validateCNPJ,
  validateEmail,
  validatePhone,
  formatPhone,
  formatDocument,
  formatZipCode,
  validatePasswordStrength,
  debounce,
  throttle,
} from './utils/validators';

export type { PasswordStrength } from './utils/validators';

export { cn } from './utils/cn';

// Constants
export { API_ENDPOINTS, VERSION } from './types';
