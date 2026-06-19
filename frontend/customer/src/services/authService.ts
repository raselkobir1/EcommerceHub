import api from '@/lib/api'
import type { ApiResponse, Customer } from '@/types'

interface AuthTokenResponse {
  accessToken: string
  refreshToken: string
  accessTokenExpiry: string
  refreshTokenExpiry: string
  userId: string
  email: string
  fullName: string
  role: string
}

export const authService = {
  login: async (email: string, password: string) => {
    const { data } = await api.post<ApiResponse<AuthTokenResponse>>('/api/auth/customers/login', { email, password })
    return data.data
  },

  register: async (payload: { fullName: string; email: string; phone: string; password: string; confirmPassword: string }) => {
    const { data } = await api.post<ApiResponse<Customer>>('/api/auth/customers/register', payload)
    return data.data
  },

  refreshToken: async (refreshToken: string) => {
    const { data } = await api.post<ApiResponse<AuthTokenResponse>>('/api/auth/customers/refresh', { refreshToken })
    return data.data
  },

  logout: async () => {
    await api.post('/api/auth/customers/logout')
  },

  getProfile: async () => {
    const { data } = await api.get<ApiResponse<Customer>>('/api/customers/profile')
    return data.data
  },
}
