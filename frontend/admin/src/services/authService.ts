import { api } from '@/lib/api'
import type { AuthTokenDto } from '@/types'

interface LoginRequest {
  email: string
  password: string
}

export const authService = {
  loginAdmin: (data: LoginRequest) =>
    api
      .post<{ data: AuthTokenDto }>('/auth/admin/login', data)
      .then((r) => r.data.data),

  logout: () => api.post('/auth/admin/logout'),

  refreshToken: (refreshToken: string) =>
    api
      .post<{ data: AuthTokenDto }>('/auth/admin/refresh-token', { refreshToken })
      .then((r) => r.data.data),
}
