'use client'
import { useEffect } from 'react'
import Link from 'next/link'
import Image from 'next/image'
import { useParams, useSearchParams, useRouter } from 'next/navigation'
import { useQuery } from '@tanstack/react-query'
import { CheckCircle, ArrowLeft, Package } from 'lucide-react'
import { orderService } from '@/services/orderService'
import { useAuthStore } from '@/stores/authStore'
import { formatBDT, formatDate } from '@/lib/utils'
import { cn } from '@/lib/utils'

const STATUS_STEPS = ['PendingPayment', 'Confirmed', 'Processing', 'Shipped', 'Delivered']

const STATUS_COLORS: Record<string, string> = {
  PendingPayment: 'bg-yellow-100 text-yellow-700',
  Confirmed: 'bg-blue-100 text-blue-700',
  Processing: 'bg-blue-100 text-blue-700',
  Shipped: 'bg-purple-100 text-purple-700',
  Delivered: 'bg-green-100 text-green-700',
  Cancelled: 'bg-red-100 text-red-700',
}

export default function OrderDetailPage() {
  const { orderNumber } = useParams<{ orderNumber: string }>()
  const searchParams = useSearchParams()
  const router = useRouter()
  const { isAuthenticated } = useAuthStore()
  const justPlaced = searchParams.get('placed') === '1'

  useEffect(() => {
    if (!isAuthenticated) router.push('/login')
  }, [isAuthenticated, router])

  const { data: order, isLoading } = useQuery({
    queryKey: ['order', orderNumber],
    queryFn: () => orderService.getOrderByNumber(orderNumber),
    enabled: isAuthenticated && !!orderNumber,
  })

  if (!isAuthenticated) return null

  if (isLoading) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6 lg:px-8 space-y-4 animate-pulse">
        <div className="h-8 w-48 bg-gray-200 rounded" />
        <div className="card p-6 space-y-3">
          {Array.from({ length: 4 }).map((_, i) => <div key={i} className="h-4 bg-gray-200 rounded" />)}
        </div>
      </div>
    )
  }

  if (!order) return <div className="py-24 text-center text-gray-400">Order not found</div>

  const currentStep = STATUS_STEPS.indexOf(order.status)

  return (
    <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6 lg:px-8 space-y-6">
      {justPlaced && (
        <div className="rounded-xl bg-green-50 border border-green-200 p-5 flex items-start gap-3">
          <CheckCircle size={24} className="text-green-500 flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-bold text-green-800">Order placed successfully!</p>
            <p className="text-sm text-green-700 mt-0.5">Thank you for your order. We&apos;ll send you updates via email.</p>
          </div>
        </div>
      )}

      <div className="flex items-center gap-3">
        <Link href="/account/orders" className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 transition-colors">
          <ArrowLeft size={18} />
        </Link>
        <div>
          <h1 className="text-xl font-bold text-gray-900">Order #{order.orderNumber}</h1>
          <p className="text-sm text-gray-500">Placed on {formatDate(order.placedAt)}</p>
        </div>
        <span className={cn('ml-auto rounded-full px-3 py-1 text-xs font-semibold', STATUS_COLORS[order.status] ?? 'bg-gray-100 text-gray-600')}>
          {order.status.replace(/([A-Z])/g, ' $1').trim()}
        </span>
      </div>

      {/* Progress */}
      {currentStep >= 0 && (
        <div className="card p-5">
          <div className="flex items-center gap-1">
            {STATUS_STEPS.map((s, i) => {
              const done = i <= currentStep
              return (
                <div key={s} className="flex flex-1 flex-col items-center">
                  <div className={cn('h-2.5 w-2.5 rounded-full', done ? 'bg-primary-600' : 'bg-gray-200')} />
                  <p className={cn('mt-1 text-center text-[10px] font-medium leading-tight', done ? 'text-primary-700' : 'text-gray-400')}>
                    {s.replace(/([A-Z])/g, ' $1').trim()}
                  </p>
                  {i < STATUS_STEPS.length - 1 && (
                    <div className={cn('absolute h-0.5 w-full -z-10', done ? 'bg-primary-300' : 'bg-gray-200')} />
                  )}
                </div>
              )
            })}
          </div>
        </div>
      )}

      {/* Items */}
      <div className="card p-5 space-y-4">
        <h2 className="font-bold text-gray-900">Items</h2>
        {order.items.map((item, i) => (
          <div key={i} className="flex gap-3">
            <div className="relative h-14 w-14 flex-shrink-0 overflow-hidden rounded-lg bg-gray-100">
              {item.imageUrl ? (
                <Image src={item.imageUrl} alt={item.productName} fill className="object-cover" sizes="56px" />
              ) : (
                <div className="flex h-full items-center justify-center"><Package size={18} className="text-gray-300" /></div>
              )}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-900 line-clamp-1">{item.productName}</p>
              <p className="text-xs text-gray-500">Qty: {item.quantity} × {formatBDT(item.unitPrice)}</p>
            </div>
            <p className="font-semibold text-gray-900 flex-shrink-0">{formatBDT(item.totalPrice)}</p>
          </div>
        ))}

        <hr />
        <div className="space-y-1 text-sm">
          <div className="flex justify-between text-gray-600"><span>Subtotal</span><span>{formatBDT(order.subtotal)}</span></div>
          {order.discountAmount > 0 && (
            <div className="flex justify-between text-green-600"><span>Discount</span><span>−{formatBDT(order.discountAmount)}</span></div>
          )}
          <div className="flex justify-between text-gray-600">
            <span>Shipping</span>
            <span>{order.shippingFee === 0 ? <span className="text-green-600">Free</span> : formatBDT(order.shippingFee)}</span>
          </div>
          <div className="flex justify-between font-bold text-gray-900 text-base pt-1">
            <span>Total</span><span>{formatBDT(order.total)}</span>
          </div>
        </div>
      </div>

      {/* Shipping address */}
      <div className="card p-5 text-sm">
        <h2 className="font-bold text-gray-900 mb-3">Delivery Address</h2>
        <p className="font-medium text-gray-800">{order.shippingAddress.recipientName}</p>
        <p className="text-gray-600">{order.shippingAddress.phone}</p>
        <p className="text-gray-600">{order.shippingAddress.addressLine1}</p>
        {order.shippingAddress.addressLine2 && <p className="text-gray-600">{order.shippingAddress.addressLine2}</p>}
        <p className="text-gray-600">{order.shippingAddress.areaThana}, {order.shippingAddress.district}, {order.shippingAddress.division}</p>
      </div>
    </div>
  )
}
