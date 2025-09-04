import { HttpClient } from '../Infra/Impl/HttpClient';
import UserService from './Impl/UserService';
const httpClientAuth = HttpClient();
httpClientAuth.init(import.meta.env.VITE_API_URL);
const userServiceImpl = UserService;
userServiceImpl.init(httpClientAuth);
const ServiceFactory = {
    UserService: userServiceImpl,
    setLogoffCallback: (cb) => {
        httpClientAuth.setLogoff(cb);
    },
};
export default ServiceFactory;
