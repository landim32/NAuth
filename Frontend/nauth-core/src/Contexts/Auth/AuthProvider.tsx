import React, { useContext, useState } from 'react';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import AuthContext from './AuthContext';
import AuthFactory from '../../Business/Factory/AuthFactory';
import AuthSession from '../../DTO/Domain/AuthSession';
import UserFactory from '../../Business/Factory/UserFactory';
import { LanguageEnum } from '../../DTO/Enum/LanguageEnum';
import IAuthProvider from './IAuthProvider';

export default function AuthProvider(props: any) {
  const [loading, setLoading] = useState(false);
  const [language, setLanguage] = useState<LanguageEnum>(LanguageEnum.English);
  const [sessionInfo, _setSessionInfo] = useState<AuthSession>(null);

  const authProviderValue: IAuthProvider = {
    loading: loading,
    language: language,
    sessionInfo: sessionInfo,

    setSession: (session: AuthSession) => {
      console.log(JSON.stringify(session));
      _setSessionInfo(session);
      AuthFactory.AuthBusiness.setSession(session);
    },
    setLanguage: (value: LanguageEnum) => {
      setLanguage(value);
    },
    loginWithEmail: async (email: string, password: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const retLog = await UserFactory.UserBusiness.loginWithEmail(email, password);
        if (retLog.sucesso) {
            authProviderValue.setSession({
              ...sessionInfo,
              userId: retLog.dataResult.user.userId,
              hash: retLog.dataResult.user.hash,
              token: retLog.dataResult.token,
              isAdmin: retLog.dataResult.user.isAdmin,
              name: retLog.dataResult.user.name,
              email: retLog.dataResult.user.email,
              language: language,
            });
            setLoading(false);
            return {
              ...ret,
              sucesso: true,
              mensagemSucesso: 'User Logged',
            };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: retLog.mensagem,
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
    logout: function (): ProviderResult {
      try {
        AuthFactory.AuthBusiness.cleanSession();
        _setSessionInfo(null);
        return {
          sucesso: true,
          mensagemErro: '',
          mensagemSucesso: '',
        };
      } catch (err) {
        return {
          sucesso: false,
          mensagemErro: 'Falha ao tenta executar o logout',
          mensagemSucesso: '',
        };
      }
    },
    loadUserSession: async () => {
      const ret = {} as Promise<ProviderResult>;
      const session = await AuthFactory.AuthBusiness.getSession();
      if (session) {
        authProviderValue.setSession(session);
        return {
          ...ret,
          sucesso: true,
        };
      }
      return {
        ...ret,
        sucesso: false,
        mensagemErro: 'Session not load',
      };
    },
  };

  return <AuthContext.Provider value={authProviderValue}>{props.children}</AuthContext.Provider>;
}
