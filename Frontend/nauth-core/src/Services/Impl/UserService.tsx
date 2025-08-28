import UserInfo from '../../DTO/Domain/UserInfo';
import StatusRequest from '../../DTO/Services/StatusRequest';
import StringResult from '../../DTO/Services/StringResult';
import UserListResult from '../../DTO/Services/UserListResult';
import UserResult from '../../DTO/Services/UserResult';
import UserTokenResult from '../../DTO/Services/UserTokenResult';
import IHttpClient from '../../Infra/Interface/IHttpClient';
import IUserService from '../Interfaces/IUserService';

const API_URL = "https://emagine.com.br/auth-api"; 
let _httpClient: IHttpClient;

const UserService: IUserService = {

  init: function (htppClient: IHttpClient): void {
    _httpClient = htppClient;
  },
  uploadImageUser: async (file: Blob, token: string) => {
    let ret = {} as StringResult;

    const formData = new FormData();
    formData.append('file', file, 'cropped.jpg');

    //formData.append("networkId", "0");
    const request = await _httpClient.doPostFormDataAuth<StringResult>(API_URL + '/uploadImageUser', formData, token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  getMe: async (token: string) => {
    let ret = {} as UserResult;
    const url = API_URL + '/getMe';
    const request = await _httpClient.doGetAuth<UserResult>(url, token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  getUserByEmail: async (email: string) => {
    let ret = {} as UserResult;
    const url = API_URL + '/getByEmail/' + email;
    const request = await _httpClient.doGet<UserResult>(url, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  getBySlug: async (slug: string) => {
    let ret = {} as UserResult;
    const url = API_URL + '/getBySlug/' + slug;
    const request = await _httpClient.doGet<UserResult>(url, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  insert: async (user: UserInfo) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doPost<UserResult>(API_URL + '/insert', user);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  update: async (user: UserInfo, token: string) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doPostAuth<UserResult>(API_URL + '/update', user, token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  loginWithEmail: async (email: string, password: string) => {
    let ret = {} as UserTokenResult;
    const request = await _httpClient.doPost<UserTokenResult>(API_URL + '/loginWithEmail', {
      email: email,
      password: password,
    });
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  hasPassword: async (token: string) => {
    let ret = {} as StatusRequest;
    const url = API_URL + '/hasPassword';
    const request = await _httpClient.doGetAuth<StatusRequest>(url, token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  changePassword: async (oldPassword: string, newPassword: string, token: string) => {
    let ret = {} as StatusRequest;
    const request = await _httpClient.doPostAuth<StatusRequest>(
      API_URL + '/changePassword',
      {
        oldPassword: oldPassword,
        newPassword: newPassword,
      },
      token,
    );
    console.log('request: ', request);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  sendRecoveryEmail: async (email: string) => {
    let ret = {} as StatusRequest;
    const url = API_URL + '/sendRecoveryMail/' + email;
    const request = await _httpClient.doGet<StatusRequest>(url, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
    let ret = {} as StatusRequest;
    const request = await _httpClient.doPost<StatusRequest>(API_URL + '/changePasswordUsingHash', {
      recoveryHash: recoveryHash,
      newPassword: newPassword,
    });
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  list: async (take: number) => {
    let ret = {} as UserListResult;
    const request = await _httpClient.doGet<UserResult>(API_URL + '/list/' + take, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  }
};

export default UserService;
