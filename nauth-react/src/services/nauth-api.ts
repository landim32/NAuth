import axios, { AxiosInstance, AxiosError } from 'axios';
import FingerprintJS from '@fingerprintjs/fingerprintjs';
import type {
  UserInfo,
  AuthSession,
  LoginCredentials,
  RegisterData,
  ChangePasswordData,
  ResetPasswordData,
  UserRole,
  ApiError,
  NAuthConfig,
} from '../types';
import { API_ENDPOINTS } from '../types';

export class NAuthAPI {
  private client: AxiosInstance;
  private config: NAuthConfig;
  private fingerprint: string | null = null;

  constructor(config: NAuthConfig) {
    this.config = {
      timeout: 30000,
      storageKey: 'nauth_token',
      storageType: 'localStorage',
      enableFingerprinting: true,
      ...config,
    };

    this.client = axios.create({
      baseURL: this.config.apiUrl,
      timeout: this.config.timeout,
      headers: {
        'Content-Type': 'application/json',
        ...this.config.headers,
      },
    });

    this.setupInterceptors();
    
    if (this.config.enableFingerprinting) {
      this.initFingerprint();
    }
  }

  private setupInterceptors(): void {
    // Request interceptor
    this.client.interceptors.request.use(
      async (config) => {
        const token = this.getToken();
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        // Add device fingerprint
        if (this.fingerprint) {
          config.headers['X-Device-Fingerprint'] = this.fingerprint;
        }

        // Add user agent
        //config.headers['User-Agent'] = navigator.userAgent;

        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        const apiError: ApiError = {
          message: error.message,
          status: error.response?.status || 500,
          errors: error.response?.data as Record<string, string[]>,
        };

        // Handle unauthorized
        if (error.response?.status === 401) {
          this.clearToken();
          if (this.config.redirectOnUnauthorized) {
            window.location.href = this.config.redirectOnUnauthorized;
          }
        }

        if (this.config.onError) {
          this.config.onError(new Error(apiError.message));
        }

        return Promise.reject(apiError);
      }
    );
  }

  private async initFingerprint(): Promise<void> {
    try {
      const fp = await FingerprintJS.load();
      const result = await fp.get();
      this.fingerprint = result.visitorId;
    } catch (error) {
      console.warn('Failed to initialize fingerprint:', error);
    }
  }

  private getStorage(): Storage {
    return this.config.storageType === 'sessionStorage'
      ? sessionStorage
      : localStorage;
  }

  private getToken(): string | null {
    return this.getStorage().getItem(this.config.storageKey || 'nauth_token');
  }

  private setToken(token: string): void {
    this.getStorage().setItem(this.config.storageKey || 'nauth_token', token);
  }

  private clearToken(): void {
    this.getStorage().removeItem(this.config.storageKey || 'nauth_token');
  }

  // Authentication Methods

  async login(credentials: LoginCredentials): Promise<AuthSession> {
    const response = await this.client.post<AuthSession>(
      API_ENDPOINTS.LOGIN,
      credentials
    );
    
    const session = response.data;
    this.setToken(session.token);
    
    if (this.config.onLogin) {
      this.config.onLogin(session.user);
    }
    
    return session;
  }

  async register(data: RegisterData): Promise<UserInfo> {
    const response = await this.client.post<UserInfo>(
      API_ENDPOINTS.REGISTER,
      data
    );
    return response.data;
  }

  logout(): void {
    this.clearToken();
    
    if (this.config.onLogout) {
      this.config.onLogout();
    }
  }

  // User Methods

  async getMe(): Promise<UserInfo> {
    const response = await this.client.get<UserInfo>(API_ENDPOINTS.GET_ME);
    return response.data;
  }

  async getUserById(userId: number): Promise<UserInfo> {
    const response = await this.client.get<UserInfo>(
      `${API_ENDPOINTS.GET_BY_ID}/${userId}`
    );
    return response.data;
  }

  async getUserByEmail(email: string): Promise<UserInfo> {
    const response = await this.client.get<UserInfo>(
      `${API_ENDPOINTS.GET_BY_EMAIL}/${email}`
    );
    return response.data;
  }

  async getUserBySlug(slug: string): Promise<UserInfo> {
    const response = await this.client.get<UserInfo>(
      `${API_ENDPOINTS.GET_BY_SLUG}/${slug}`
    );
    return response.data;
  }

  async updateUser(data: Partial<UserInfo>): Promise<UserInfo> {
    const response = await this.client.post<UserInfo>(
      API_ENDPOINTS.UPDATE_USER,
      data
    );
    return response.data;
  }

  async uploadImage(file: File): Promise<string> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await this.client.post<string>(
      API_ENDPOINTS.UPLOAD_IMAGE,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  }

  async hasPassword(): Promise<boolean> {
    const response = await this.client.get<boolean>(API_ENDPOINTS.HAS_PASSWORD);
    return response.data;
  }

  async changePassword(data: ChangePasswordData): Promise<void> {
    await this.client.post(API_ENDPOINTS.CHANGE_PASSWORD, data);
  }

  async sendRecoveryEmail(email: string): Promise<void> {
    await this.client.get(`${API_ENDPOINTS.SEND_RECOVERY_EMAIL}/${email}`);
  }

  async resetPassword(data: ResetPasswordData): Promise<void> {
    await this.client.post(API_ENDPOINTS.RESET_PASSWORD, data);
  }

  async listUsers(take: number = 50): Promise<UserInfo[]> {
    const response = await this.client.get<UserInfo[]>(
      `${API_ENDPOINTS.LIST_USERS}/${take}`
    );
    return response.data;
  }

  // Role Methods

  async listRoles(take: number = 50): Promise<UserRole[]> {
    const response = await this.client.get<UserRole[]>(
      `${API_ENDPOINTS.LIST_ROLES}/${take}`
    );
    return response.data;
  }

  async getRoleById(roleId: number): Promise<UserRole> {
    const response = await this.client.get<UserRole>(
      `${API_ENDPOINTS.GET_ROLE_BY_ID}/${roleId}`
    );
    return response.data;
  }

  async getRoleBySlug(slug: string): Promise<UserRole> {
    const response = await this.client.get<UserRole>(
      `${API_ENDPOINTS.GET_ROLE_BY_SLUG}/${slug}`
    );
    return response.data;
  }

  // Utility Methods

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getCurrentToken(): string | null {
    return this.getToken();
  }

  setAuthToken(token: string): void {
    this.setToken(token);
  }

  clearAuth(): void {
    this.clearToken();
  }
}

// Factory function to create API client
export function createNAuthClient(config: NAuthConfig): NAuthAPI {
  return new NAuthAPI(config);
}
