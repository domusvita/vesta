import { useAuth0 } from '@auth0/auth0-react'
import { Outlet, Link } from 'react-router-dom'
import useUserStore from '../store/userStore'

export default function MainLayout() {
  const { isAuthenticated, loginWithRedirect, logout } = useAuth0()
  const user = useUserStore((s) => s.user)
  const isAdmin = user?.roles?.some(role => role.name === 'Admin')

  return (
    <div>
      <nav>
        {isAuthenticated ? (
          <>
            <span>{user?.displayName}</span>
            {isAdmin && (
              <Link to="/admin" style={{ marginLeft: '1rem', marginRight: '1rem' }}>
                Admin: Users
              </Link>
            )}
            <button onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}>
              Logout
            </button>
          </>
        ) : (
          <button onClick={() => loginWithRedirect()}>Login</button>
        )}
      </nav>
      <main>
        <Outlet />
      </main>
    </div>
  )
}
