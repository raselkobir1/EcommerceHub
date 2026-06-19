import { useState } from 'react'
import { Link, Outlet, useLocation } from 'react-router-dom'
import {
  LayoutDashboard, Package, Tag, ShoppingCart, Warehouse, Truck,
  TicketPercent, Star, BarChart2, Settings, ChevronDown,
  ChevronRight, LogOut, Bell, Menu, X, User,
} from 'lucide-react'
import { useAuthStore } from '@/stores/authStore'
import { useLogout } from '@/hooks/useAuth'
import { cn } from '@/lib/utils'

interface NavItem {
  label: string
  path?: string
  icon: React.ReactNode
  children?: { label: string; path: string }[]
}

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/dashboard', icon: <LayoutDashboard size={18} /> },
  { label: 'Products', path: '/products', icon: <Package size={18} /> },
  { label: 'Categories', path: '/categories', icon: <Tag size={18} /> },
  { label: 'Orders', path: '/orders', icon: <ShoppingCart size={18} /> },
  { label: 'Inventory', path: '/inventory', icon: <Warehouse size={18} /> },
  { label: 'Suppliers', path: '/suppliers', icon: <Truck size={18} /> },
  {
    label: 'Promotions',
    icon: <TicketPercent size={18} />,
    children: [
      { label: 'Coupons', path: '/promotions/coupons' },
      { label: 'Banners', path: '/promotions/banners' },
    ],
  },
  { label: 'Reviews', path: '/reviews', icon: <Star size={18} /> },
  { label: 'Reports', path: '/reports', icon: <BarChart2 size={18} /> },
  { label: 'Settings', path: '/settings', icon: <Settings size={18} /> },
]

function NavLink({ item }: { item: NavItem }) {
  const { pathname } = useLocation()
  const [open, setOpen] = useState(false)

  if (item.children) {
    const isActive = item.children.some((c) => pathname.startsWith(c.path))
    return (
      <div>
        <button
          onClick={() => setOpen(!open)}
          className={cn(
            'flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
            isActive ? 'bg-primary-700 text-white' : 'text-gray-300 hover:bg-gray-700 hover:text-white'
          )}
        >
          {item.icon}
          <span className="flex-1 text-left">{item.label}</span>
          {open ? <ChevronDown size={14} /> : <ChevronRight size={14} />}
        </button>
        {open && (
          <div className="ml-8 mt-1 space-y-1">
            {item.children.map((child) => (
              <Link
                key={child.path}
                to={child.path}
                className={cn(
                  'block rounded-lg px-3 py-2 text-sm transition-colors',
                  pathname === child.path
                    ? 'bg-primary-600 text-white'
                    : 'text-gray-400 hover:bg-gray-700 hover:text-white'
                )}
              >
                {child.label}
              </Link>
            ))}
          </div>
        )}
      </div>
    )
  }

  const isActive = pathname === item.path || pathname.startsWith(item.path! + '/')
  return (
    <Link
      to={item.path!}
      className={cn(
        'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
        isActive ? 'bg-primary-600 text-white' : 'text-gray-300 hover:bg-gray-700 hover:text-white'
      )}
    >
      {item.icon}
      {item.label}
    </Link>
  )
}

export default function DashboardLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const user = useAuthStore((s) => s.user)
  const logout = useLogout()
  const { pathname } = useLocation()

  const flatItems = navItems.flatMap<{ label: string; path?: string }>((i) =>
    i.children ? i.children : [{ label: i.label, path: i.path }]
  )
  const pageTitle = flatItems.find((i) => i.path && pathname.startsWith(i.path))?.label ?? 'Dashboard'

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 z-20 bg-black/50 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={cn(
          'fixed inset-y-0 left-0 z-30 flex w-64 flex-col bg-gray-900 transition-transform duration-300 lg:static lg:translate-x-0',
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        <div className="flex h-16 items-center justify-between px-4">
          <span className="text-lg font-bold text-white">EcommerceHub</span>
          <button onClick={() => setSidebarOpen(false)} className="text-gray-400 lg:hidden">
            <X size={20} />
          </button>
        </div>

        <nav className="flex-1 overflow-y-auto px-3 py-4 space-y-1">
          {navItems.map((item) => (
            <NavLink key={item.label} item={item} />
          ))}
        </nav>

        <div className="border-t border-gray-700 p-4">
          <div className="flex items-center gap-3 mb-3">
            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary-600 text-white text-sm font-semibold">
              {user?.fullName?.[0]?.toUpperCase() ?? 'A'}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-white truncate">{user?.fullName ?? 'Admin'}</p>
              <p className="text-xs text-gray-400 truncate">{user?.role ?? ''}</p>
            </div>
          </div>
          <button
            onClick={logout}
            className="flex w-full items-center gap-2 rounded-lg px-3 py-2 text-sm text-gray-400 hover:bg-gray-700 hover:text-white transition-colors"
          >
            <LogOut size={16} />
            Sign out
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="flex flex-1 flex-col overflow-hidden">
        {/* Top bar */}
        <header className="flex h-16 items-center justify-between border-b bg-white px-4 lg:px-6">
          <div className="flex items-center gap-3">
            <button onClick={() => setSidebarOpen(true)} className="text-gray-500 lg:hidden">
              <Menu size={22} />
            </button>
            <h1 className="text-lg font-semibold text-gray-900">{pageTitle}</h1>
          </div>
          <div className="flex items-center gap-3">
            <button className="relative rounded-full p-2 text-gray-500 hover:bg-gray-100">
              <Bell size={20} />
            </button>
            <div className="flex items-center gap-2 text-sm text-gray-700">
              <User size={18} />
              <span className="hidden sm:block">{user?.email ?? ''}</span>
            </div>
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto p-4 lg:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
