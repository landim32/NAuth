import React from 'react';
import ReactDOM from 'react-dom/client';
import './bootstrap.css';
import 'react-loading-skeleton/dist/skeleton.css';
import App from './App';
import { BrowserRouter } from "react-router-dom";
import './i18n'; // Import i18next configuration
import ScrollToTop from './Components/ScrollToTop';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter basename={process.env.REACT_APP_SITE_BASENAME}>
      <ScrollToTop />
      <React.Suspense fallback="loading...">
        <App />
      </React.Suspense>
    </BrowserRouter>
  </React.StrictMode>
);
