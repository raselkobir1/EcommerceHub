'use client'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { ShoppingCart, User, Search, Menu, X, ChevronDown } from 'lucide-react'
import { useState } from 'react'
import { useCartStore } from '@/stores/cartStore'
import { useAuthStore } from '@/stores/authStore'
import { cn } from '@/lib/utils'

const NAV_LINKS = [
  { label: 'Home', href: '/' },
  { label: 'Products', href: '/products' },
]

export default function Navbar() {
  const pathname = usePathname()
  const [mobileOpen, setMobileOpen] = useState(false)
  const [userOpen, setUserOpen] = useState(false)
  const itemCount = useCartStore((s) => s.itemCount())
  const { customer, isAuthenticated, logout } = useAuthStore()

  return (
    <header className="sticky top-0 z-50 bg-white border-b shadow-sm">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between gap-4">
          {/* Logo */}
          <Link href="/" className="flex-shrink-0 text-xl font-bold text-primary-700">
            EcommerceHub
          </Link>

          {/* Search */}
          <div className="hidden flex-1 max-w-md sm:flex items-center rounded-lg border bg-gray-50 px-3 py-2 gap-2">
            <Search size={16} className="text-gray-400" />
            <input
              placeholder="Search products…"
              className="flex-1 bg-transparent text-sm placeholder-gray-400 focus:outline-none"
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  const q = (e.target as HTMLInputElement).value.trim()
                  if (q) window.location.href = `/products?search=${encodeURIComponent(q)}`
                }
              }}
            />
          </div>

          {/* Desktop Nav */}
          <nav className="hidden lg:flex items-center gap-6">
            {NAV_LINKS.map((link) => (
              <Link
                key={link.href}
                href={link.href}
                className={cn('text-sm font-medium transition-colors', pathname === link.href ? 'text-primary-600' : 'text-gray-600 hover:text-gray-900')}
              >
                {link.label}
              </Link>
            ))}
          </nav>

          {/* Right icons */}
          <div className="flex items-center gap-2">
            <Link href="/cart" className="relative rounded-lg p-2 text-gray-600 hover:bg-gray-100 transition-colors">
              <ShoppingCart size={20} />
              {itemCount > 0 && (
                <span className="absolute -top-0.5 -right-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-red-500 text-[10px] font-bold text-white">
                  {itemCount > 9 ? '9+' : itemCount}
                </span>
              )}
            </Link>

            {isAuthenticated ? (
              <div className="relative">
                <button
                  onClick={() => setUserOpen(!userOpen)}
                  className="flex items-center gap-1.5 rounded-lg px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors"
                >
                  <User size={16} />
                  <span className="hidden sm:inline">{customer?.fullName?.split(' ')[0]}</span>
                  <ChevronDown size={14} />
                </button>
                {userOpen && (
                  <div className="absolute right-0 mt-1 w-48 rounded-xl border bg-white shadow-lg py-1 z-10">
                    <Link href="/account" onClick={() => setUserOpen(false)} className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50">My Account</Link>
                    <Link href="/account/orders" onClick={() => setUserOpen(false)} className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50">My Orders</Link>
                    <hr className="my-1" />
                    <button onClick={() => { logout(); setUserOpen(false) }} className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50">Sign Out</button>
                  </div>
                )}
              </div>
            ) : (
              <Link href="/login" className="hidden sm:flex items-center gap-1.5 rounded-lg px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100 transition-colors">
                <User size={16} />
                Sign In
              </Link>
            )}

            <button onClick={() => setMobileOpen(!mobileOpen)} className="rounded-lg p-2 text-gray-600 hover:bg-gray-100 transition-colors lg:hidden">
              {mobileOpen ? <X size={20} /> : <Menu size={20} />}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile menu */}
      {mobileOpen && (
        <div className="lg:hidden border-t bg-white px-4 py-4 space-y-2">
          {/* Mobile search */}
          <div className="flex items-center rounded-lg border bg-gray-50 px-3 py-2 gap-2 mb-4">
            <Search size={16} className="text-gray-400" />
            <input placeholder="Search products…" className="flex-1 bg-transparent text-sm placeholder-gray-400 focus:outline-none" />
          </div>
          {NAV_LINKS.map((link) => (
            <Link key={link.href} href={link.href} onClick={() => setMobileOpen(false)}
              className={cn('block rounded-lg px-3 py-2 text-sm font-medium', pathname === link.href ? 'bg-primary-50 text-primary-700' : 'text-gray-700 hover:bg-gray-50')}>
              {link.label}
            </Link>
          ))}
          {!isAuthenticated && (
            <Link href="/login" onClick={() => setMobileOpen(false)} className="block rounded-lg px-3 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50">
              Sign In
            </Link>
          )}
        </div>
      )}
    </header>
  )
}
