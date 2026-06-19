'use client'
import { useEffect } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { User, Package, MapPin, LogOut } from 'lucide-react'
import { useAuthStore } from '@/stores/authStore'

export default function AccountPage() {
  const router = useRouter()
  const { customer, isAuthenticated, logout } = useAuthStore()

  useEffect(() => {
    if (!isAuthenticated) router.push('/login')
  }, [isAuthenticated, router])

  if (!isAuthenticated) return null

  const LINKS = [
    { icon: Package, label: 'My Orders', desc: 'Track and manage your orders', href: '/account/orders' },
    { icon: MapPin, label: 'Addresses', desc: 'Manage delivery addresses', href: '/account/addresses' },
  ]

  return (
    <div className="mx-auto max-w-2xl px-4 py-8 sm:px-6 lg:px-8 space-y-6">
      {/* Profile card */}
      <div className="card p-6 flex items-center gap-4">
        <div className="flex h-16 w-16 items-center justify-center rounded-full bg-primary-100">
          <User size={28} className="text-primary-600" />
        </div>
        <div>
          <h1 className="text-xl font-bold text-gray-900">{customer?.fullName}</h1>
          <p className="text-sm text-gray-500">{customer?.email}</p>
        </div>
      </div>

      {/* Nav cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        {LINKS.map(({ icon: Icon, label, desc, href }) => (
          <Link key={href} href={href} className="card p-5 flex items-center gap-4 hover:shadow-md transition-shadow">
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
      <button onClick={() => { logout(); router.push('/') }}
        className="flex w-full items-center gap-3 rounded-xl border border-red-200 p-4 text-red-600 hover:bg-red-50 transition-colors">
        <LogOut size={18} />
        <span className="font-medium">Sign Out</span>
      </button>
    </div>
  )
}
