import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Theme } from '@radix-ui/themes'
import MainLayout from './layouts/MainLayout'
import ProtectedRoute from './components/ProtectedRoute'
import AdminRoute from './components/AdminRoute'
import HomePage from './pages/HomePage'
import ProfileSetupPage from './pages/ProfileSetupPage'
import NotFoundPage from './pages/NotFoundPage'
import AdminUsersPage from './pages/admin/AdminUsersPage'
import useCurrentUser from './hooks/useCurrentUser'

function AppRoutes() {
  useCurrentUser()

  return (
    <Routes>
      <Route path="/setup" element={<ProfileSetupPage />} />
      <Route element={<ProtectedRoute />}>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<HomePage />} />
          <Route path="admin" element={<AdminRoute />}>
            <Route index element={<AdminUsersPage />} />
          </Route>
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Route>
    </Routes>
  )
}

export default function App() {
  return (
    <Theme appearance="light" accentColor="indigo" radius="medium">
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </Theme>
  )
}
