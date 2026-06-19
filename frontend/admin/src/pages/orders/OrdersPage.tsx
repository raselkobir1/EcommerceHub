import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { orderService } from '@/services/orderService'
import { OrderStatus } from '@/types'
import Badge from '@/components/ui/Badge'
import Pagination from '@/components/ui/Pagination'
import { formatBDT, formatDateTime } from '@/lib/utils'

const TABS = ['All', 'Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled']
const STATUS_BADGE: Record<string, 'default' | 'success' | 'warning' | 'info' | 'danger'> = {
  Pending: 'default', Confirmed: 'info', Processing: 'warning',
  Shipped: 'info', Delivered: 'success', Cancelled: 'danger',
}

export default function OrdersPage() {
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState('All')

  const { data, isLoading } = useQuery({
    queryKey: ['orders', page, status],
    queryFn: () => orderService.getOrders({
      page,
      pageSize: 20,
      status: status === 'All' ? '' : status as OrderStatus,
    }),
  })

  return (
    <div className="space-y-4">
      {/* Status tabs */}
      <div className="flex gap-1 overflow-x-auto pb-1">
        {TABS.map((tab) => (
          <button
            key={tab}
            onClick={() => { setStatus(tab); setPage(1) }}
            className={`flex-shrink-0 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
              status === tab ? 'bg-primary-600 text-white' : 'text-gray-600 hover:bg-gray-100'
            }`}
          >
            {tab}
          </button>
        ))}
      </div>

      <div className="bg-white rounded-xl border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-gray-50">
                <th className="px-4 py-3 text-left font-medium text-gray-500">Order #</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Customer</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Status</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Payment</th>
                <th className="px-4 py-3 text-right font-medium text-gray-500">Total</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Date</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {isLoading && Array.from({ length: 8 }).map((_, i) => (
                <tr key={i}>
                  {Array.from({ length: 6 }).map((_, j) => (
                    <td key={j} className="px-4 py-3">
                      <div className="h-4 rounded bg-gray-200 animate-pulse" />
                    </td>
                  ))}
                </tr>
              ))}
              {!isLoading && data?.items.map((order) => (
                <tr key={order.id} className="hover:bg-gray-50 transition-colors cursor-pointer">
                  <td className="px-4 py-3 font-mono text-xs text-primary-600 font-semibold">{order.orderNumber}</td>
                  <td className="px-4 py-3 text-gray-700">{order.customerName}</td>
                  <td className="px-4 py-3">
                    <Badge variant={STATUS_BADGE[order.status] ?? 'default'}>
                      {order.status}
                    </Badge>
                  </td>
                  <td className="px-4 py-3">
                    <Badge variant={order.paymentStatus === 'Paid' ? 'success' : 'warning'}>
                      {order.paymentStatus}
                    </Badge>
                  </td>
                  <td className="px-4 py-3 text-right font-semibold">{formatBDT(order.grandTotal)}</td>
                  <td className="px-4 py-3 text-gray-500 text-xs">{formatDateTime(order.createdAt)}</td>
                </tr>
              ))}
              {!isLoading && !data?.items.length && (
                <tr><td colSpan={6} className="px-4 py-10 text-center text-gray-400">No orders found</td></tr>
              )}
            </tbody>
          </table>
        </div>

        {data && data.totalPages > 1 && (
          <div className="border-t px-4 py-3 flex justify-end">
            <Pagination currentPage={page} totalPages={data.totalPages} onPageChange={setPage} />
          </div>
        )}
      </div>
    </div>
  )
}
