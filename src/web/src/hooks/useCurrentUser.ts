import { useEffect } from 'react'
import { useAuth0 } from '@auth0/auth0-react'
import { useNavigate } from 'react-router-dom'
import { getMe } from '../services/userService'
import { setAuthToken } from '../services/apiClient'
import useUserStore from '../store/userStore'

export default function useCurrentUser() {
  const { isAuthenticated, getAccessTokenSilently } = useAuth0()
  const setUser = useUserStore((s) => s.setUser)
  const navigate = useNavigate()

  useEffect(() => {
    if (!isAuthenticated) return

    async function bootstrap() {
      const token = await getAccessTokenSilently()
      setAuthToken(token)

      const user = await getMe().catch((err) => {
        if (err?.response?.status === 404) return null
        throw err
      })

      console.log('Current user:', user)

      if (user) {
        setUser(user)
        if (user.roles.includes('Admin')) {
          navigate('/admin')
        }
      } else {
        navigate('/setup')
      }
    }

    bootstrap()
  }, [isAuthenticated])
}
