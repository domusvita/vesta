import { useEffect, useRef } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import { Outlet } from 'react-router-dom'

export default function ProtectedRoute() {
  const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0()
  const didRedirect = useRef(false)

  useEffect(() => {
    if (!isLoading && !isAuthenticated && !didRedirect.current) {
      didRedirect.current = true
      void loginWithRedirect()
    }
  }, [isAuthenticated, isLoading, loginWithRedirect])

  if (isLoading || !isAuthenticated) {
    return null
  }

  return <Outlet />
}
