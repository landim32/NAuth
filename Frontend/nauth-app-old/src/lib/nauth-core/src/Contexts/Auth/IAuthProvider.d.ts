import ProviderResult from '../../DTO/Contexts/ProviderResult';
import AuthSession from '../../DTO/Domain/AuthSession';
import { LanguageEnum } from '../../DTO/Enum/LanguageEnum';
interface IAuthProvider {
    loading: boolean;
    sessionInfo: AuthSession;
    language: LanguageEnum;
    setLanguage: (value: LanguageEnum) => void;
    setSession: (session: AuthSession) => void;
    loginWithEmail: (email: string, password: string) => Promise<ProviderResult>;
    logout: () => ProviderResult;
    loadUserSession: () => Promise<ProviderResult>;
}
export default IAuthProvider;
