import React from 'react';
import { renderHook, act } from '@testing-library/react-hooks';
import UserProvider from './UserProvider';
import UserContext from './UserContext';
import UserFactory from '../../Business/Factory/UserFactory';
import UserInfo from '../../DTO/Domain/UserInfo';
import { LanguageEnum } from '../../DTO/Enum/LanguageEnum';
import AuthSession from '../../DTO/Domain/AuthSession';

jest.mock('../../Business/Factory/UserFactory', () => ({
  __esModule: true,
  default: {
    UserBusiness: {
      loginWithEmail: jest.fn(),
      setSession: jest.fn(),
    },
  },
}));

describe('UserContext loginWithEmail', () => {
  const loginMock = UserFactory.UserBusiness.loginWithEmail as jest.Mock;

  beforeEach(() => {
    loginMock.mockReset();
  });

  const wrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
    <UserProvider>{children}</UserProvider>
  );

  it('should login and update user and session', async () => {
    const fakeUser: UserInfo = {
      userId: 1,
      email: 'user@example.com',
      slug: 'user-slug',
      imageUrl: '',
      name: 'User',
      hash: 'hash',
      password: '',
      isAdmin: true,
      birthDate: '',
      idDocument: '',
      pixKey: '',
      phones: [],
      addresses: [],
      createAt: '',
      updateAt: '',
    };

    loginMock.mockResolvedValue({
      sucesso: true,
      mensagem: '',
      dataResult: { token: 'abc', user: fakeUser },
    });

    const { result } = renderHook(() => React.useContext(UserContext), { wrapper });

    act(() => {
      result.current.setSession({} as AuthSession);
    });

    let response: any;
    await act(async () => {
      response = await result.current.loginWithEmail('user@example.com', 'secret');
    });

    expect(loginMock).toHaveBeenCalledWith('user@example.com', 'secret');
    expect(response.sucesso).toBe(true);
    expect(result.current.user).toEqual({ token: 'abc', user: fakeUser });
    expect(result.current.sessionInfo).toEqual({
      userId: fakeUser.userId,
      hash: fakeUser.hash,
      token: 'abc',
      isAdmin: fakeUser.isAdmin,
      name: fakeUser.name,
      email: fakeUser.email,
      language: LanguageEnum.English,
    });
    expect(result.current.loading).toBe(false);
  });

  it('should handle login failure', async () => {
    loginMock.mockResolvedValue({
      sucesso: false,
      mensagem: 'Invalid',
    });

    const { result } = renderHook(() => React.useContext(UserContext), { wrapper });

    act(() => {
      result.current.setSession({} as AuthSession);
    });

    let response: any;
    await act(async () => {
      response = await result.current.loginWithEmail('user@example.com', 'secret');
    });

    expect(loginMock).toHaveBeenCalledWith('user@example.com', 'secret');
    expect(response.sucesso).toBe(false);
    expect(result.current.user).toBeNull();
    expect(result.current.sessionInfo).toEqual({});
    expect(result.current.loading).toBe(false);
  });
});
