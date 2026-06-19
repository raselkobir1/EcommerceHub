'use client'
import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { CheckCircle, ChevronRight } from 'lucide-react'
import { useCartStore } from '@/stores/cartStore'
import { useAuthStore } from '@/stores/authStore'
import { orderService } from '@/services/orderService'
import { formatBDT } from '@/lib/utils'

const BD_PHONE = /^(\+8801|01)[3-9]\d{8}$/

const addressSchema = z.object({
  recipientName: z.string().min(2, 'Required'),
  phone: z.string().regex(BD_PHONE, 'Invalid BD phone number'),
  addressLine1: z.string().min(5, 'Address is required'),
  addressLine2: z.string().optional(),
  areaThana: z.string().min(2, 'Required'),
  district: z.string().min(2, 'Required'),
  division: z.string().min(2, 'Required'),
})

type AddressData = z.infer<typeof addressSchema>

const PAYMENT_METHODS = [
  { value: 'bKash', label: 'bKash', desc: 'Pay with bKash mobile banking' },
  { value: 'Nagad', label: 'Nagad', desc: 'Pay with Nagad' },
  { value: 'SSLCommerz', label: 'SSLCommerz', desc: 'Card / Net Banking' },
  { value: 'COD', label: 'Cash on Delivery', desc: 'Pay when you receive' },
]

const BD_DIVISIONS = ['Dhaka', 'Chittagong', 'Rajshahi', 'Khulna', 'Barishal', 'Sylhet', 'Rangpur', 'Mymensingh']

export default function CheckoutPage() {
  const router = useRouter()
  const { items, subtotal, clearCart } = useCartStore()
  const { isAuthenticated } = useAuthStore()
  const [step, setStep] = useState<'address' | 'payment' | 'review'>('address')
  const [paymentMethod, setPaymentMethod] = useState('COD')
  const [isPlacing, setIsPlacing] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [addressData, setAddressData] = useState<AddressData | null>(null)

  const { register, handleSubmit, formState: { errors }, getValues } = useForm<AddressData>({
    resolver: zodResolver(addressSchema),
  })

  if (!isAuthenticated) {
    return (
      <div className="flex min-h-[50vh] flex-col items-center justify-center gap-4 text-center">
        <h2 className="text-xl font-bold text-gray-900">Sign in to continue</h2>
        <p className="text-gray-500">You need to be signed in to place an order</p>
        <button onClick={() => router.push('/login')} className="btn-primary">Sign In</button>
      </div>
    )
  }

  if (items.length === 0) {
    router.push('/cart')
    return null
  }

  const shippingFee = subtotal() >= 999 ? 0 : 60
  const total = subtotal() + shippingFee

  const onAddressSubmit = (data: AddressData) => {
    setAddressData(data)
    setStep('payment')
  }

  const onPlaceOrder = async () => {
    if (!addressData) return
    setIsPlacing(true)
    setError(null)
    try {
      const result = await orderService.placeOrder({
        items: items.map((i) => ({ productId: i.productId, variantId: i.variantId, quantity: i.quantity })),
        shippingAddress: addressData,
        paymentMethod,
      })
      clearCart()
      router.push(`/account/orders/${result.orderNumber}?placed=1`)
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
      setError(msg ?? 'Failed to place order. Please try again.')
      setIsPlacing(false)
    }
  }

  const STEPS = [
    { key: 'address', label: 'Delivery' },
    { key: 'payment', label: 'Payment' },
    { key: 'review', label: 'Review' },
  ]

  return (
    <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6 lg:px-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-8">Checkout</h1>

      {/* Steps */}
      <div className="flex items-center gap-2 mb-8">
        {STEPS.map((s, i) => {
          const isActive = s.key === step
          const isDone = STEPS.findIndex((x) => x.key === step) > i
          return (
            <div key={s.key} className="flex items-center gap-2">
              <div className={`flex h-7 w-7 items-center justify-center rounded-full text-xs font-bold ${isDone ? 'bg-green-500 text-white' : isActive ? 'bg-primary-600 text-white' : 'bg-gray-200 text-gray-500'}`}>
                {isDone ? <CheckCircle size={14} /> : i + 1}
              </div>
              <span className={`text-sm font-medium ${isActive ? 'text-primary-600' : 'text-gray-500'}`}>{s.label}</span>
              {i < STEPS.length - 1 && <ChevronRight size={16} className="text-gray-300 mx-1" />}
            </div>
          )
        })}
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          {/* Address Step */}
          {step === 'address' && (
            <form onSubmit={handleSubmit(onAddressSubmit)} className="card p-6 space-y-4">
              <h2 className="text-base font-bold text-gray-900">Delivery Address</h2>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Recipient Name</label>
                  <input {...register('recipientName')} className="input" />
                  {errors.recipientName && <p className="mt-1 text-xs text-red-600">{errors.recipientName.message}</p>}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Phone</label>
                  <input {...register('phone')} placeholder="01XXXXXXXXX" className="input" />
                  {errors.phone && <p className="mt-1 text-xs text-red-600">{errors.phone.message}</p>}
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Address Line 1</label>
                <input {...register('addressLine1')} placeholder="House, Road, Block" className="input" />
                {errors.addressLine1 && <p className="mt-1 text-xs text-red-600">{errors.addressLine1.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Address Line 2 (optional)</label>
                <input {...register('addressLine2')} placeholder="Landmark, near…" className="input" />
              </div>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Area / Thana</label>
                  <input {...register('areaThana')} placeholder="Dhanmondi" className="input" />
                  {errors.areaThana && <p className="mt-1 text-xs text-red-600">{errors.areaThana.message}</p>}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">District</label>
                  <input {...register('district')} placeholder="Dhaka" className="input" />
                  {errors.district && <p className="mt-1 text-xs text-red-600">{errors.district.message}</p>}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Division</label>
                  <select {...register('division')} className="input">
                    <option value="">Select…</option>
                    {BD_DIVISIONS.map((d) => <option key={d} value={d}>{d}</option>)}
                  </select>
                  {errors.division && <p className="mt-1 text-xs text-red-600">{errors.division.message}</p>}
                </div>
              </div>
              <button type="submit" className="btn-primary">Continue to Payment</button>
            </form>
          )}

          {/* Payment Step */}
          {step === 'payment' && (
            <div className="card p-6 space-y-4">
              <h2 className="text-base font-bold text-gray-900">Payment Method</h2>
              <div className="space-y-3">
                {PAYMENT_METHODS.map((m) => (
                  <label key={m.value} className={`flex cursor-pointer items-start gap-3 rounded-xl border p-4 transition-colors ${paymentMethod === m.value ? 'border-primary-500 bg-primary-50' : 'border-gray-200 hover:border-gray-300'}`}>
                    <input type="radio" value={m.value} checked={paymentMethod === m.value}
                      onChange={(e) => setPaymentMethod(e.target.value)} className="mt-0.5 accent-primary-600" />
                    <div>
                      <p className="font-semibold text-gray-900">{m.label}</p>
                      <p className="text-sm text-gray-500">{m.desc}</p>
                    </div>
                  </label>
                ))}
              </div>
              <div className="flex gap-3">
                <button onClick={() => setStep('address')} className="btn-outline">Back</button>
                <button onClick={() => setStep('review')} className="btn-primary">Review Order</button>
              </div>
            </div>
          )}

          {/* Review Step */}
          {step === 'review' && addressData && (
            <div className="card p-6 space-y-5">
              <h2 className="text-base font-bold text-gray-900">Review Your Order</h2>

              <div className="rounded-lg bg-gray-50 p-4 text-sm">
                <p className="font-semibold text-gray-700 mb-1">Delivery to:</p>
                <p className="text-gray-600">{addressData.recipientName} · {addressData.phone}</p>
                <p className="text-gray-600">{addressData.addressLine1}, {addressData.areaThana}, {addressData.district}, {addressData.division}</p>
              </div>

              <div className="rounded-lg bg-gray-50 p-4 text-sm">
                <p className="font-semibold text-gray-700 mb-1">Payment: {paymentMethod}</p>
              </div>

              {error && <div className="rounded-lg bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-700">{error}</div>}

              <div className="flex gap-3">
                <button onClick={() => setStep('payment')} className="btn-outline">Back</button>
                <button onClick={onPlaceOrder} disabled={isPlacing} className="btn-primary flex-1">
                  {isPlacing ? 'Placing Order…' : `Place Order · ${formatBDT(total)}`}
                </button>
              </div>
            </div>
          )}
        </div>

        {/* Order Summary */}
        <div className="card p-5 h-fit space-y-4">
          <h2 className="text-sm font-bold text-gray-900">Order Summary</h2>
          <div className="space-y-2 text-sm">
            {items.map((item) => (
              <div key={`${item.productId}_${item.variantId}`} className="flex justify-between gap-2">
                <span className="text-gray-600 line-clamp-1">{item.productName} ×{item.quantity}</span>
                <span className="flex-shrink-0 font-medium">{formatBDT(item.price * item.quantity)}</span>
              </div>
            ))}
            <hr />
            <div className="flex justify-between text-gray-600">
              <span>Subtotal</span><span>{formatBDT(subtotal())}</span>
            </div>
            <div className="flex justify-between text-gray-600">
              <span>Shipping</span>
              <span>{shippingFee === 0 ? <span className="text-green-600 font-medium">Free</span> : formatBDT(shippingFee)}</span>
            </div>
            <hr />
            <div className="flex justify-between font-bold text-gray-900">
              <span>Total</span><span>{formatBDT(total)}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
