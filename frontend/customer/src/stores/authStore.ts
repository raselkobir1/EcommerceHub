'use client'
import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { Customer } from '@/types'

interface AuthState {
  customer: Customer | null
  accessToken: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  setTokens: (access: string, refresh: string) => void
  setCustomer: (customer: Customer) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      customer: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      setTokens: (access, refresh) => {
        if (typeof window !== 'undefined') {
          localStorage.setItem('customer_access_token', access)
          localStorage.setItem('customer_refresh_token', refresh)
        }
        set({ accessToken: access, refreshToken: refresh, isAuthenticated: true })
      },
      setCustomer: (customer) => set({ customer }),
      logout: () => {
        if (typeof window !== 'undefined') {
          localStorage.removeItem('customer_access_token')
          localStorage.removeItem('customer_refresh_token')
        }
        set({ customer: null, accessToken: null, refreshToken: null, isAuthenticated: false })
      },
    }),
    {
      name: 'customer-auth',
      partialize: (s) => ({ customer: s.customer, accessToken: s.accessToken, refreshToken: s.refreshToken, isAuthenticated: s.isAuthenticated }),
    }
  )
)
