const API_URL = "https://emagine.com.br/auth-api";
let _httpClient;
const UserService = {
    init: function (htppClient) {
        _httpClient = htppClient;
    },
    uploadImageUser: async (file, token) => {
        let ret = {};
        const formData = new FormData();
        formData.append('file', file, 'cropped.jpg');
        //formData.append("networkId", "0");
        const request = await _httpClient.doPostFormDataAuth(API_URL + '/uploadImageUser', formData, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    getMe: async (token) => {
        let ret = {};
        const url = API_URL + '/getMe';
        const request = await _httpClient.doGetAuth(url, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    getUserByEmail: async (email) => {
        let ret = {};
        const url = API_URL + '/getByEmail/' + email;
        const request = await _httpClient.doGet(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    getBySlug: async (slug) => {
        let ret = {};
        const url = API_URL + '/getBySlug/' + slug;
        const request = await _httpClient.doGet(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    insert: async (user) => {
        let ret = {};
        const request = await _httpClient.doPost(API_URL + '/insert', user);
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    update: async (user, token) => {
        let ret = {};
        const request = await _httpClient.doPostAuth(API_URL + '/update', user, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    loginWithEmail: async (email, password) => {
        let ret = {};
        const request = await _httpClient.doPost(API_URL + '/loginWithEmail', {
            email: email,
            password: password,
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    hasPassword: async (token) => {
        let ret = {};
        const url = API_URL + '/hasPassword';
        const request = await _httpClient.doGetAuth(url, token);
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    changePassword: async (oldPassword, newPassword, token) => {
        let ret = {};
        const request = await _httpClient.doPostAuth(API_URL + '/changePassword', {
            oldPassword: oldPassword,
            newPassword: newPassword,
        }, token);
        console.log('request: ', request);
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    sendRecoveryEmail: async (email) => {
        let ret = {};
        const url = API_URL + '/sendRecoveryMail/' + email;
        const request = await _httpClient.doGet(url, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    changePasswordUsingHash: async (recoveryHash, newPassword) => {
        let ret = {};
        const request = await _httpClient.doPost(API_URL + '/changePasswordUsingHash', {
            recoveryHash: recoveryHash,
            newPassword: newPassword,
        });
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    },
    list: async (take) => {
        let ret = {};
        const request = await _httpClient.doGet(API_URL + '/list/' + take, {});
        if (request.success) {
            return request.data;
        }
        else {
            ret = Object.assign(Object.assign({}, ret), { mensagem: request.messageError, sucesso: false });
        }
        return ret;
    }
};
export default UserService;
