import UserInfo from '../../DTO/Domain/UserInfo';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import UrlProviderResult from '../../DTO/Contexts/UrlProviderResult';
import UserProviderResult from '../../DTO/Contexts/UserProviderResult';

interface IUserProvider {
  loading: boolean;
  loadingList: boolean;
  loadingPassword: boolean;
  loadingUpdate: boolean;
  loadingSearch: boolean;

  userHasPassword: boolean;
  user: UserInfo;
  users: UserInfo[];

  //searchResult: UserListPagedInfo;

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
  //search: (networkId: number, keyword: string, pageNum: number, profileId?: number) => Promise<ProviderResult>;
}

export default IUserProvider;
