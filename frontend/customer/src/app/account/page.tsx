'use client'
import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { useEffect } from 'react'
import { User, Package, MapPin, LogOut, CheckCircle, Clock } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { useAuthStore } from '@/stores/authStore'
import { authService } from '@/services/authService'

export default function AccountPage() {
  const router = useRouter()
  const { isAuthenticated, setCustomer, logout } = useAuthStore()

  useEffect(() => {
    if (!isAuthenticated) router.push('/login')
  }, [isAuthenticated, router])

  const { data: profile, isLoading, isError } = useQuery({
    queryKey: ['customerProfile'],
    queryFn: authService.getProfile,
    enabled: isAuthenticated,
    staleTime: 5 * 60 * 1000,
  })

  // Keep the auth store in sync whenever the server returns fresh data
  useEffect(() => {
    if (profile) setCustomer(profile)
  }, [profile, setCustomer])

  if (!isAuthenticated) return null

  const LINKS = [
    { icon: Package, label: 'My Orders',  desc: 'Track and manage your orders',  href: '/account/orders'    },
    { icon: MapPin,   label: 'Addresses',  desc: 'Manage delivery addresses',     href: '/account/addresses' },
  ]

  const memberSince = profile?.createdAt
    ? new Date(profile.createdAt).toLocaleDateString('en-GB', { month: 'long', year: 'numeric' })
    : null

  return (
    <div className="mx-auto max-w-2xl px-4 py-8 sm:px-6 lg:px-8 space-y-6">
      {/* Profile card */}
      <div className="card p-6 flex items-center gap-4">
        <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-full bg-primary-100">
          <User size={28} className="text-primary-600" />
        </div>

        {isLoading ? (
          <div className="space-y-2 flex-1">
            <div className="h-5 w-40 animate-pulse rounded bg-gray-200" />
            <div className="h-4 w-56 animate-pulse rounded bg-gray-100" />
          </div>
        ) : isError ? (
          <p className="text-sm text-red-500">Could not load profile. Please refresh.</p>
        ) : (
          <div className="flex-1 min-w-0">
            <h1 className="text-xl font-bold text-gray-900 truncate">{profile?.fullName}</h1>
            <p className="text-sm text-gray-500 truncate">{profile?.email}</p>

            <div className="mt-2 flex flex-wrap gap-3">
              {/* Email verification badge */}
              {profile?.isEmailVerified !== undefined && (
                profile.isEmailVerified ? (
                  <span className="inline-flex items-center gap-1 rounded-full bg-green-50 px-2 py-0.5 text-xs font-medium text-green-700">
                    <CheckCircle size={12} />
                    Email verified
                  </span>
                ) : (
                  <span className="inline-flex items-center gap-1 rounded-full bg-amber-50 px-2 py-0.5 text-xs font-medium text-amber-700">
                    <Clock size={12} />
                    Email not verified
                  </span>
                )
              )}

              {/* Phone */}
              {profile?.phone && (
                <span className="text-xs text-gray-400">{profile.phone}</span>
              )}

              {/* Member since */}
              {memberSince && (
                <span className="text-xs text-gray-400">Member since {memberSince}</span>
              )}
            </div>
          </div>
        )}
      </div>

      {/* Nav cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        {LINKS.map(({ icon: Icon, label, desc, href }) => (
          <Link
            key={href}
            href={href}
            className="card p-5 flex items-center gap-4 hover:shadow-md transition-shadow"
          >
            <div className="rounded-xl bg-primary-50 p-3">
              <Icon size={20} className="text-primary-600" />
            </div>
            <div>
              <p className="font-semibold text-gray-900">{label}</p>
              <p className="text-sm text-gray-500">{desc}</p>
            </div>
          </Link>
        ))}
      </div>

      {/* Sign out */}
      <button
        onClick={() => { logout(); router.push('/') }}
        className="flex w-full items-center gap-3 rounded-xl border border-red-200 p-4 text-red-600 hover:bg-red-50 transition-colors"
      >
        <LogOut size={18} />
        <span className="font-medium">Sign Out</span>
      </button>
    </div>
  )
}
