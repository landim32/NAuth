import './App.css';
import { Routes, Route, Outlet, Link } from "react-router-dom";
import ContextBuilder from './NAuth/Contexts/Utils/ContextBuilder';
import AuthProvider from './NAuth/Contexts/Auth/AuthProvider';
import UserPage from './NAuth/Pages/UserPage';
import PasswordPage from './NAuth/Pages/PasswordPage';
import LoginPage from './NAuth/Pages/LoginPage';
import RecoveryPage from './NAuth/Pages/RecoveryPage';
import UserProvider from './NAuth/Contexts/User/UserProvider';
import HomePage from './Pages/HomePage';
import Error404Page from './NAuth/Pages/Error404Page';
import UserFullPage from './NAuth/Pages/UserFullPage';
import Menu from './Pages/HomePage/Menu';

function Layout() {
  return (
    <div>
      <Menu />
      <Outlet />
    </div>
  );
}

function App() {
  const ContextContainer = ContextBuilder([AuthProvider, UserProvider]);

  return (
    <ContextContainer>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route path="new-seller" element={<UserFullPage />} />
          <Route path="account">
            <Route index element={<LoginPage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="edit-account" element={<UserPage />} />
            <Route path="new-account" element={<UserPage />} />
            <Route path="recovery-password" element={<RecoveryPage />} />
            <Route path="change-password" element={<PasswordPage />} />
          </Route>
          
        </Route>
        <Route path="*" element={<Error404Page />} />
      </Routes>
    </ContextContainer>
  );
}

export default App;
