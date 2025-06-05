import React from 'react';
import IAuthProvider from './IAuthProvider';

const AuthContext = React.createContext<IAuthProvider>(null);

export default AuthContext;