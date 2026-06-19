import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { authService } from '@/services/authService'
import { useAuthStore } from '@/stores/authStore'

export function useLogin() {
  const navigate = useNavigate()
  const { setTokens, setUser } = useAuthStore()

  return useMutation({
    mutationFn: authService.loginAdmin,
    onSuccess: (data) => {
      setTokens(data.accessToken, data.refreshToken)
      setUser({
        id: data.userId,
        email: data.email,
        fullName: data.fullName,
        role: data.role,
      })
      navigate('/dashboard')
    },
  })
}

export function useLogout() {
  const navigate = useNavigate()
  const logout = useAuthStore((s) => s.logout)

  return () => {
    logout()
    navigate('/login')
  }
}
