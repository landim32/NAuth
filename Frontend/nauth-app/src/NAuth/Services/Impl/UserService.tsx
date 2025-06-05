import UserInfo from "../../DTO/Domain/UserInfo";
import StatusRequest from "../../DTO/Services/StatusRequest";
import StringResult from "../../DTO/Services/StringResult";
import UserListResult from "../../DTO/Services/UserListResult";
import UserResult from "../../DTO/Services/UserResult";
import UserTokenResult from "../../DTO/Services/UserTokenResult";
import IHttpClient from "../../Infra/Interface/IHttpClient"; 
import IUserService from "../Interfaces/IUserService";

let _httpClient : IHttpClient;

const UserService : IUserService = {
    init: function (htppClient: IHttpClient): void {
        _httpClient = htppClient;
    },
    uploadImageUser: async (file: Blob, token: string) => {
            let ret: StringResult;
    
            const formData = new FormData();
            formData.append('file', file, 'cropped.jpg');
    
            //formData.append("networkId", "0");
            let request = await _httpClient.doPostFormDataAuth<StringResult>("/uploadImageUser", formData, token);
            if (request.success) {
                return request.data;
            }
            else {
                ret = {
                    mensagem: request.messageError,
                    sucesso: false,
                    ...ret
                };
            }
            return ret;
        },
    getMe: async (token: string) => {
        let ret: UserResult;
        let url = "/getme";
        let request = await _httpClient.doGetAuth<UserResult>(url, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getUserByEmail: async (email: string) => {
        let ret: UserResult;
        let url = "/getbyemail/" + email;
        let request = await _httpClient.doGet<UserResult>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getBySlug: async (slug: string) => {
        let ret: UserResult;
        let url = "/getBySlug/" + slug;
        let request = await _httpClient.doGet<UserResult>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    getTokenAuthorized: async (email: string, password: string) => {
        let ret: UserTokenResult;
        let request = await _httpClient.doPost<UserTokenResult>("/gettokenauthorized", {
            email: email,
            password: password
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    insert: async (user: UserInfo) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("/insert", user);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    update: async (user: UserInfo, token: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPostAuth<UserResult>("/update", user, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    loginWithEmail: async (email: string, password: string) => {
        let ret: UserResult;
        let request = await _httpClient.doPost<UserResult>("/loginwithemail", {
            email: email,
            password: password
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    hasPassword: async (token: string) => {
        let ret: StatusRequest;
        let url = "/haspassword";
        let request = await _httpClient.doGetAuth<StatusRequest>(url, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;        
    },
    changePassword: async (oldPassword: string, newPassword: string, token: string) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPostAuth<StatusRequest>("/changepassword", {
            oldPassword: oldPassword,
            newPassword: newPassword
        }, token);
        console.log("request: ", request);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    sendRecoveryEmail: async (email: string) => {
        let ret: StatusRequest;
        let url = "/sendrecoveryemail/" + email;
        let request = await _httpClient.doGet<StatusRequest>(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
        let ret: StatusRequest;
        let request = await _httpClient.doPost<StatusRequest>("/changepasswordusinghash", {
            recoveryHash: recoveryHash,
            newPassword: newPassword
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    },
    list: async (take: number) => {
        let ret: UserListResult;
        let request = await _httpClient.doGet<UserResult>("/list/" + take, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    }
    /*,
    search: async (networkId: number, keyword: string, pageNum: number, token: string, profileId?: number) => {
        let ret: UserListPagedResult;
        let param: UserSearchParam;
        param = {
            ...param,
            networkId: networkId,
            keyword: keyword,
            profileId: profileId,
            pageNum: pageNum
        };
        let request = await _httpClient.doPostAuth<UserListPagedResult>("/User/search", param, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = {
                mensagem: request.messageError,
                sucesso: false,
                ...ret
            };
        }
        return ret;
    }
        */
}

export default UserService;