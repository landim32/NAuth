import React from 'react';
import IUserProvider from './IUserProvider';

const UserContext = React.createContext<IUserProvider>(null);

export default UserContext;