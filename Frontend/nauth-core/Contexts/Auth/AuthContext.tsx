import React from 'react';
import IAuthProvider from './IAuthProvider';

const AuthContext = React.createContext<IAuthProvider>(null as unknown as IAuthProvider);

export default AuthContext;
