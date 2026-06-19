import { useRef, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { orderService } from '@/services/orderService'
import { OrderStatus, OrderSummaryDto } from '@/types'
import Badge from '@/components/ui/Badge'
import Pagination from '@/components/ui/Pagination'
import { formatBDT, formatDateTime } from '@/lib/utils'

const TABS = ['All', 'Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled']

const STATUS_BADGE: Record<string, 'default' | 'success' | 'warning' | 'info' | 'danger'> = {
  Pending: 'default',
  Confirmed: 'info',
  Processing: 'warning',
  Shipped: 'info',
  Delivered: 'success',
  Cancelled: 'danger',
  Returned: 'danger',
  Refunded: 'warning',
}

// Allowed forward transitions per status (keeps the dropdown focused)
const NEXT_STATUSES: Record<OrderStatus, OrderStatus[]> = {
  Pending: ['Confirmed', 'Cancelled'],
  Confirmed: ['Processing', 'Cancelled'],
  Processing: ['Shipped', 'Cancelled'],
  Shipped: ['Delivered', 'Returned'],
  Delivered: ['Returned', 'Refunded'],
  Cancelled: [],
  Returned: ['Refunded'],
  Refunded: [],
}

// ─── Status update dropdown ────────────────────────────────────────────────────

interface StatusDropdownProps {
  order: OrderSummaryDto
}

function StatusDropdown({ order }: StatusDropdownProps) {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const containerRef = useRef<HTMLDivElement>(null)

  const nextStatuses = NEXT_STATUSES[order.status] ?? []

  const mutation = useMutation({
    mutationFn: (newStatus: OrderStatus) =>
      orderService.updateOrderStatus(order.id, newStatus),
    onSuccess: () => {
      setError(null)
      setOpen(false)
      queryClient.invalidateQueries({ queryKey: ['orders'] })
    },
    onError: () => {
      setError('Update failed')
    },
  })

  if (nextStatuses.length === 0) {
    return <span className="text-xs text-gray-400 italic">No actions</span>
  }

  return (
    <div ref={containerRef} className="relative inline-block">
      <button
        disabled={mutation.isPending}
        onClick={() => { setError(null); setOpen((v) => !v) }}
        className="inline-flex items-center gap-1 rounded-md border border-gray-300 bg-white px-2.5 py-1 text-xs font-medium text-gray-700 shadow-sm hover:bg-gray-50 disabled:opacity-50 transition-colors"
      >
        {mutation.isPending ? (
          <>
            <span className="inline-block h-3 w-3 animate-spin rounded-full border-2 border-gray-400 border-t-transparent" />
            Updating…
          </>
        ) : (
          <>
            Update status
            <svg className="h-3 w-3 text-gray-400" viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
            </svg>
          </>
        )}
      </button>

      {error && (
        <p className="absolute left-0 top-full mt-0.5 text-xs text-red-500 whitespace-nowrap">
          {error}
        </p>
      )}

      {open && !mutation.isPending && (
        <>
          {/* Backdrop to close on outside click */}
          <div
            className="fixed inset-0 z-10"
            onClick={() => setOpen(false)}
          />
          <div className="absolute left-0 top-full z-20 mt-1 w-36 rounded-lg border bg-white py-1 shadow-lg">
            {nextStatuses.map((s) => (
              <button
                key={s}
                onClick={() => mutation.mutate(s)}
                className="flex w-full items-center gap-2 px-3 py-2 text-left text-xs text-gray-700 hover:bg-gray-50"
              >
                <span
                  className={`inline-block h-2 w-2 rounded-full ${
                    s === 'Cancelled' || s === 'Returned' || s === 'Refunded'
                      ? 'bg-red-400'
                      : 'bg-emerald-400'
                  }`}
                />
                {s}
              </button>
            ))}
          </div>
        </>
      )}
    </div>
  )
}

// ─── Orders page ──────────────────────────────────────────────────────────────

export default function OrdersPage() {
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState('All')

  const { data, isLoading } = useQuery({
    queryKey: ['orders', page, status],
    queryFn: () =>
      orderService.getOrders({
        page,
        pageSize: 20,
        status: status === 'All' ? '' : (status as OrderStatus),
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
              status === tab
                ? 'bg-primary-600 text-white'
                : 'text-gray-600 hover:bg-gray-100'
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
                <th className="px-4 py-3 text-left font-medium text-gray-500">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {isLoading &&
                Array.from({ length: 8 }).map((_, i) => (
                  <tr key={i}>
                    {Array.from({ length: 7 }).map((_, j) => (
                      <td key={j} className="px-4 py-3">
                        <div className="h-4 rounded bg-gray-200 animate-pulse" />
                      </td>
                    ))}
                  </tr>
                ))}

              {!isLoading &&
                data?.items.map((order) => (
                  <tr key={order.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3 font-mono text-xs text-primary-600 font-semibold">
                      {order.orderNumber}
                    </td>
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
                    <td className="px-4 py-3 text-right font-semibold">
                      {formatBDT(order.grandTotal)}
                    </td>
                    <td className="px-4 py-3 text-gray-500 text-xs">
                      {formatDateTime(order.createdAt)}
                    </td>
                    <td className="px-4 py-3">
                      <StatusDropdown order={order} />
                    </td>
                  </tr>
                ))}

              {!isLoading && !data?.items.length && (
                <tr>
                  <td colSpan={7} className="px-4 py-10 text-center text-gray-400">
                    No orders found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {data && data.totalPages > 1 && (
          <div className="border-t px-4 py-3 flex justify-end">
            <Pagination
              currentPage={page}
              totalPages={data.totalPages}
              onPageChange={setPage}
            />
          </div>
        )}
      </div>
    </div>
  )
}
