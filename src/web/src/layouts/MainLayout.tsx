import { useAuth0 } from '@auth0/auth0-react'
import { Outlet, Link, useLocation } from 'react-router-dom'
import { Flex } from '@radix-ui/themes'
import useUserStore from '../store/userStore'

export default function MainLayout() {
  const { isAuthenticated, loginWithRedirect, logout } = useAuth0()
  const user = useUserStore((s) => s.user)
  const isAdmin = user?.roles?.some(role => role.name === 'Admin')
  const location = useLocation()
  const isAdminSection = location.pathname.startsWith('/admin')

  return (
    <div>
      <nav style={{ borderBottom: '1px solid #e5e7eb', padding: '1rem' }}>
        <Flex align="center" justify="between">
          <Link to="/" style={{ textDecoration: 'none', fontWeight: 'bold', fontSize: '1.1rem' }}>
            Vesta
          </Link>
          <Flex align="center" gap="4">
            {isAuthenticated ? (
              <>
                <span>{user?.displayName}</span>
                {isAdmin && (
                  <Flex align="center" gap="2">
                    <Link to="/admin" style={{ textDecoration: 'none', color: isAdminSection ? '#6366f1' : 'inherit' }}>
                      Admin
                    </Link>
                    {isAdminSection && (
                      <Flex align="center" gap="2" style={{ paddingLeft: '0.5rem', borderLeft: '1px solid #e5e7eb' }}>
                        <Link 
                          to="/admin" 
                          style={{ 
                            textDecoration: 'none', 
                            color: location.pathname === '/admin' ? '#6366f1' : 'inherit',
                            fontSize: '0.9rem'
                          }}
                        >
                          Users
                        </Link>
                        <Link 
                          to="/admin/questions" 
                          style={{ 
                            textDecoration: 'none', 
                            color: location.pathname === '/admin/questions' ? '#6366f1' : 'inherit',
                            fontSize: '0.9rem'
                          }}
                        >
                          Questions
                        </Link>
                      </Flex>
                    )}
                  </Flex>
                )}
                <button onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}>
                  Logout
                </button>
              </>
            ) : (
              <button onClick={() => loginWithRedirect()}>Login</button>
            )}
          </Flex>
        </Flex>
      </nav>
      <main style={{ padding: '1rem' }}>
        <Outlet />
      </main>
    </div>
  )
}
