'use client'
import { useEffect, useState } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { useQuery } from '@tanstack/react-query'
import { Package } from 'lucide-react'
import { orderService } from '@/services/orderService'
import { useAuthStore } from '@/stores/authStore'
import { formatBDT, formatDate } from '@/lib/utils'
import { cn } from '@/lib/utils'

const STATUS_COLORS: Record<string, string> = {
  PendingPayment: 'bg-yellow-100 text-yellow-700',
  Confirmed: 'bg-blue-100 text-blue-700',
  Processing: 'bg-blue-100 text-blue-700',
  Shipped: 'bg-purple-100 text-purple-700',
  Delivered: 'bg-green-100 text-green-700',
  Cancelled: 'bg-red-100 text-red-700',
  Return: 'bg-gray-100 text-gray-700',
}

export default function OrdersPage() {
  const router = useRouter()
  const { isAuthenticated } = useAuthStore()
  const [page, setPage] = useState(1)

  useEffect(() => {
    if (!isAuthenticated) router.push('/login')
  }, [isAuthenticated, router])

  const { data, isLoading } = useQuery({
    queryKey: ['my-orders', page],
    queryFn: () => orderService.getMyOrders({ page, pageSize: 10 }),
    enabled: isAuthenticated,
  })

  if (!isAuthenticated) return null

  return (
    <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6 lg:px-8 space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">My Orders</h1>

      {isLoading && (
        <div className="space-y-4">
          {Array.from({ length: 3 }).map((_, i) => (
            <div key={i} className="card p-5 animate-pulse">
              <div className="flex justify-between">
                <div className="h-4 w-32 bg-gray-200 rounded" />
                <div className="h-6 w-20 bg-gray-200 rounded-full" />
              </div>
              <div className="mt-3 h-4 w-48 bg-gray-200 rounded" />
            </div>
          ))}
        </div>
      )}

      {!isLoading && !data?.items.length && (
        <div className="flex flex-col items-center gap-4 py-16 text-center">
          <Package size={56} className="text-gray-300" />
          <h2 className="text-lg font-semibold text-gray-900">No orders yet</h2>
          <Link href="/products" className="btn-primary">Start Shopping</Link>
        </div>
      )}

      <div className="space-y-4">
        {(data?.items ?? []).map((order) => (
          <Link key={order.id} href={`/account/orders/${order.orderNumber}`} className="card p-5 block hover:shadow-md transition-shadow">
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="font-semibold text-gray-900">#{order.orderNumber}</p>
                <p className="text-sm text-gray-500 mt-0.5">{formatDate(order.placedAt)} · {order.items.length} item{order.items.length > 1 ? 's' : ''}</p>
              </div>
              <span className={cn('rounded-full px-3 py-1 text-xs font-semibold', STATUS_COLORS[order.status] ?? 'bg-gray-100 text-gray-600')}>
                {order.status.replace(/([A-Z])/g, ' $1').trim()}
              </span>
            </div>
            <div className="mt-3 flex items-center justify-between">
              <p className="text-sm text-gray-600">{order.paymentMethod}</p>
              <p className="font-bold text-gray-900">{formatBDT(order.total)}</p>
            </div>
          </Link>
        ))}
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex justify-center gap-2">
          <button disabled={page <= 1} onClick={() => setPage(page - 1)} className="btn-outline">Previous</button>
          <span className="flex items-center px-4 text-sm text-gray-600">Page {page} of {data.totalPages}</span>
          <button disabled={page >= data.totalPages} onClick={() => setPage(page + 1)} className="btn-outline">Next</button>
        </div>
      )}
    </div>
  )
}
