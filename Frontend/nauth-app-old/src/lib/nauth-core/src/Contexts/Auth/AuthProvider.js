import { jsx as _jsx } from "react/jsx-runtime";
import React, { useState } from 'react';
import AuthContext from './AuthContext';
import AuthFactory from '../../Business/Factory/AuthFactory';
import UserFactory from '../../Business/Factory/UserFactory';
import { LanguageEnum } from '../../DTO/Enum/LanguageEnum';
export default function AuthProvider(props) {
    const [loading, setLoading] = useState(false);
    const [language, setLanguage] = useState(LanguageEnum.English);
    const [sessionInfo, _setSessionInfo] = useState(null);
    const authProviderValue = {
        loading: loading,
        language: language,
        sessionInfo: sessionInfo,
        setSession: (session) => {
            console.log(JSON.stringify(session));
            _setSessionInfo(session);
            AuthFactory.AuthBusiness.setSession(session);
        },
        setLanguage: (value) => {
            setLanguage(value);
        },
        loginWithEmail: async (email, password) => {
            const ret = {};
            setLoading(true);
            try {
                const retLog = await UserFactory.UserBusiness.loginWithEmail(email, password);
                if (retLog.sucesso) {
                    authProviderValue.setSession(Object.assign(Object.assign({}, sessionInfo), { userId: retLog.dataResult.user.userId, hash: retLog.dataResult.user.hash, token: retLog.dataResult.token, isAdmin: retLog.dataResult.user.isAdmin, name: retLog.dataResult.user.name, email: retLog.dataResult.user.email, language: language }));
                    setLoading(false);
                    return Object.assign(Object.assign({}, ret), { sucesso: true, mensagemSucesso: 'User Logged' });
                }
                else {
                    setLoading(false);
                    return Object.assign(Object.assign({}, ret), { sucesso: false, mensagemErro: retLog.mensagem });
                }
            }
            catch (err) {
                setLoading(false);
                return Object.assign(Object.assign({}, ret), { sucesso: false, mensagemErro: JSON.stringify(err) });
            }
        },
        logout: function () {
            try {
                AuthFactory.AuthBusiness.cleanSession();
                _setSessionInfo(null);
                return {
                    sucesso: true,
                    mensagemErro: '',
                    mensagemSucesso: '',
                };
            }
            catch (err) {
                return {
                    sucesso: false,
                    mensagemErro: 'Falha ao tenta executar o logout',
                    mensagemSucesso: '',
                };
            }
        },
        loadUserSession: async () => {
            const ret = {};
            const session = await AuthFactory.AuthBusiness.getSession();
            if (session) {
                authProviderValue.setSession(session);
                return Object.assign(Object.assign({}, ret), { sucesso: true });
            }
            return Object.assign(Object.assign({}, ret), { sucesso: false, mensagemErro: 'Session not load' });
        },
    };
    return _jsx(AuthContext.Provider, { value: authProviderValue, children: props.children });
}
