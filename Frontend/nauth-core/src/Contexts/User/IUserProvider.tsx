import UserInfo from '../../DTO/Domain/UserInfo';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import UrlProviderResult from '../../DTO/Contexts/UrlProviderResult';
import UserProviderResult from '../../DTO/Contexts/UserProviderResult';
import AuthSession from '../../DTO/Domain/AuthSession';
import { LanguageEnum } from '../../DTO/Enum/LanguageEnum';

interface IUserProvider {
  loading: boolean;
  loadingList: boolean;
  loadingPassword: boolean;
  loadingUpdate: boolean;
  loadingSearch: boolean;

  userHasPassword: boolean;
  sessionInfo: AuthSession;
  language: LanguageEnum;
  user: UserInfo;
  users: UserInfo[];

  setLanguage: (value: LanguageEnum) => void;
  setSession: (session: AuthSession) => void;

  logout: () => ProviderResult;
  loadUserSession: () => Promise<ProviderResult>;

  setUser: (user: UserInfo) => void;
  uploadImageUser: (file: Blob) => Promise<UrlProviderResult>;
  getMe: () => Promise<UserProviderResult>;
  getUserByEmail: (email: string) => Promise<ProviderResult>;
  getBySlug: (slug: string) => Promise<ProviderResult>;
  insert: (user: UserInfo) => Promise<ProviderResult>;
  update: (user: UserInfo) => Promise<ProviderResult>;
  loginWithEmail: (email: string, password: string) => Promise<ProviderResult>;

  hasPassword: () => Promise<ProviderResult>;
  changePassword: (oldPassword: string, newPassword: string) => Promise<ProviderResult>;
  sendRecoveryEmail: (email: string) => Promise<ProviderResult>;
  changePasswordUsingHash: (recoveryHash: string, newPassword: string) => Promise<ProviderResult>;

  list: (take: number) => Promise<ProviderResult>;
}

export default IUserProvider;
