import { useEffect } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Layout } from '@/components/layout/Layout';
import { RequireAuth, RequireAdmin } from '@/components/common/RouteGuards';
import { bootstrapAuth } from '@/hooks/useAuth';

import { HomePage } from '@/pages/HomePage';
import { WatchPage } from '@/pages/WatchPage';
import { SearchPage } from '@/pages/SearchPage';
import { ChannelPage } from '@/pages/ChannelPage';
import { LikedPage } from '@/pages/LikedPage';
import { HistoryPage } from '@/pages/HistoryPage';
import { SubscriptionsPage } from '@/pages/SubscriptionsPage';
import { StudioPage } from '@/pages/StudioPage';
import { SettingsPage } from '@/pages/SettingsPage';
import { AdminPage } from '@/pages/AdminPage';
import { LoginPage } from '@/pages/LoginPage';
import { RegisterPage } from '@/pages/RegisterPage';
import { ConfirmEmailPage } from '@/pages/ConfirmEmailPage';
import { ForgotPasswordPage } from '@/pages/ForgotPasswordPage';
import { ResetPasswordPage } from '@/pages/ResetPasswordPage';
import { NotFoundPage } from '@/pages/NotFoundPage';

export function App() {
  useEffect(() => {
    bootstrapAuth();
  }, []);

  return (
    <BrowserRouter>
      <Routes>
        {/* Auth pages render outside the main Layout (no header/sidebar chrome). */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/confirm-email" element={<ConfirmEmailPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />

        <Route element={<Layout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/watch/:id" element={<WatchPage />} />
          <Route path="/search" element={<SearchPage />} />
          <Route path="/channel/:channelId" element={<ChannelPage />} />

          <Route element={<RequireAuth />}>
            <Route path="/liked" element={<LikedPage />} />
            <Route path="/history" element={<HistoryPage />} />
            <Route path="/subscriptions" element={<SubscriptionsPage />} />
            <Route path="/studio" element={<StudioPage />} />
            <Route path="/settings" element={<SettingsPage />} />
          </Route>

          <Route element={<RequireAdmin />}>
            <Route path="/admin" element={<AdminPage />} />
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
