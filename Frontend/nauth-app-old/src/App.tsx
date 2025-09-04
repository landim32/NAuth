import './App.css';
import { Routes, Route, Outlet, Link } from "react-router-dom";
import HomePage from './Pages/HomePage';
import Menu from './Pages/HomePage/Menu';
import UserFullPage from './Pages/UserFullPage';
import LoginPage from './Pages/LoginPage';
import UserPage from './Pages/UserPage';
import RecoveryPage from './Pages/RecoveryPage';
import PasswordPage from './Pages/PasswordPage';
import Error404Page from './Pages/Error404Page';
import { AuthProvider, ContextBuilder, UserProvider } from './lib/nauth-core';

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
