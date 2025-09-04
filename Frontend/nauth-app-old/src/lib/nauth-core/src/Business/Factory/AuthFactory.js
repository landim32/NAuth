import AuthBusiness from '../Impl/AuthBusiness';
const authBusinessImpl = AuthBusiness;
const AuthFactory = {
    AuthBusiness: authBusinessImpl,
};
export default AuthFactory;
