const LS_KEY = 'login-with-metamask:auth';
const AuthBusiness = {
    getSession: () => {
        const ls = window.localStorage.getItem(LS_KEY);
        return ls && JSON.parse(ls);
    },
    setSession: (session) => {
        console.log('Set Session: ', JSON.stringify(session));
        localStorage.setItem(LS_KEY, JSON.stringify(session));
    },
    cleanSession: () => {
        localStorage.removeItem(LS_KEY);
    },
};
export default AuthBusiness;
