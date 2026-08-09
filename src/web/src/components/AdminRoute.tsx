import { Navigate, Outlet } from 'react-router-dom'
import useUserStore from '../store/userStore'

export default function AdminRoute() {
  const user = useUserStore((s) => s.user)

  if (!user) {
    return null
  }

  if (!user.roles.some((r) => r.name === 'Admin')) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
