import { useState } from 'react';
import IUserProvider from './IUserProvider';
import UserContext from './UserContext';
import UserInfo from '../../DTO/Domain/UserInfo';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import UserFactory from '../../Business/Factory/UserFactory';
import UserProviderResult from '../../DTO/Contexts/UserProviderResult';
import UrlProviderResult from '../../DTO/Contexts/UrlProviderResult';

export default function UserProvider(props: any) {
  const [loading, setLoading] = useState<boolean>(false);
  const [loadingList, setLoadingList] = useState<boolean>(false);
  const [loadingPassword, setLoadingPassword] = useState<boolean>(false);
  const [loadingUpdate, setLoadingUpdate] = useState<boolean>(false);
  const [loadingSearch, setLoadingSearch] = useState<boolean>(false);

  const [userHasPassword, setUserHasPassword] = useState<boolean>(false);

  const [userImage, setUserImage] = useState<string>('');
  const [user, _setUser] = useState<UserInfo>(null);
  const [users, setUsers] = useState<UserInfo[]>([]);
  //const [searchResult, setSearchResult] = useState<UserListPagedInfo>(null);

  const userProviderValue: IUserProvider = {
    loading: loading,
    loadingList: loadingList,
    loadingPassword: loadingPassword,
    loadingUpdate: loadingUpdate,
    loadingSearch: loadingSearch,
    userHasPassword: userHasPassword,
    user: user,
    users: users,
    //searchResult: searchResult,
    setUser: (user: UserInfo) => {
      _setUser(user);
    },
    uploadImageUser: async (file: Blob) => {
      const ret = {} as Promise<UrlProviderResult>;
      setLoading(true);
      //try {
      const brt = await UserFactory.UserBusiness.uploadImageUser(file);
      if (brt.sucesso) {
        setLoading(false);
        setUserImage(brt.dataResult);
        return {
          ...ret,
          sucesso: true,
          url: brt.dataResult,
          mensagemSucesso: 'Profile added',
        };
      } else {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: brt.mensagem,
        };
      }
      /*
                    }
                    catch (err) {
                        setLoadingUpdate(false);
                        return {
                            ...ret,
                            sucesso: false,
                            mensagemErro: JSON.stringify(err)
                        };
                    }
                    */
    },
    getMe: async () => {
      const ret = {} as Promise<UserProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.getMe();
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            user: brt.dataResult,
            sucesso: true,
            mensagemSucesso: 'User load',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            user: null,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          user: null,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    getUserByEmail: async (email: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.getUserByEmail(email);
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User load',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    getBySlug: async (slug: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.getBySlug(slug);
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User load',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    insert: async (user: UserInfo) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.insert(user);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User inseted',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    update: async (user: UserInfo) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.update(user);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User updated',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    loginWithEmail: async (email: string, password: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.update(user);
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User updated',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    hasPassword: async () => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingPassword(true);
      setUserHasPassword(false);
      try {
        const brt = await UserFactory.UserBusiness.hasPassword();
        if (brt.sucesso) {
          setUserHasPassword(true);
          setLoadingPassword(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'Password changed',
          };
        } else {
          setLoadingPassword(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingPassword(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    changePassword: async (oldPassword: string, newPassword: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.changePassword(oldPassword, newPassword);
        console.log('changePassword: ', JSON.stringify(brt));
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: brt.mensagem,
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        console.log('Error change password: ', err);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    sendRecoveryEmail: async (email: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.sendRecoveryEmail(email);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'Recovery email sent successfully',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.changePasswordUsingHash(recoveryHash, newPassword);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'Recovery email sent successfully',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    list: async (take: number) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingList(true);
      try {
        const brt = await UserFactory.UserBusiness.list(take);
        if (brt.sucesso) {
          setLoadingList(false);
          setUsers(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
          };
        } else {
          setLoadingList(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingList(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    /*,
        search: async (networkId: number, keyword: string, pageNum: number, profileId?: number) => {
            let ret = {} as Promise<ProviderResult>;
            setLoadingSearch(true);
            setSearchResult(null);
                let brt = await UserFactory.UserBusiness.search(networkId, keyword, pageNum, profileId);
                if (brt.sucesso) {
                    setLoadingSearch(false);
                    setSearchResult(brt.dataResult);
                    return {
                        ...ret,
                        sucesso: true,
                        mensagemSucesso: "Search executed"
                    };
                }
                else {
                    setLoadingSearch(false);
                    return {
                        ...ret,
                        sucesso: false,
                        mensagemErro: brt.mensagem
                    };
                }
        }
                */
  };

  return <UserContext.Provider value={userProviderValue}>{props.children}</UserContext.Provider>;
}
