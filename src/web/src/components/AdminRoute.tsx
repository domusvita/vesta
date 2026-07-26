import { Navigate, Outlet } from 'react-router-dom'
import useUserStore from '../store/userStore'

export default function AdminRoute() {
  const user = useUserStore((s) => s.user)

  if (!user?.roles.includes('Admin')) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
