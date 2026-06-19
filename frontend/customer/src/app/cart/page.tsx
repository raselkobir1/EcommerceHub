'use client'
import Link from 'next/link'
import Image from 'next/image'
import { Trash2, ShoppingBag } from 'lucide-react'
import { useCartStore } from '@/stores/cartStore'
import { formatBDT } from '@/lib/utils'

export default function CartPage() {
  const { items, removeItem, updateQuantity, subtotal } = useCartStore()

  if (items.length === 0) {
    return (
      <div className="flex min-h-[50vh] flex-col items-center justify-center gap-4 px-4 text-center">
        <ShoppingBag size={64} className="text-gray-300" />
        <h2 className="text-xl font-bold text-gray-900">Your cart is empty</h2>
        <p className="text-gray-500">Add some products to get started</p>
        <Link href="/products" className="btn-primary">Continue Shopping</Link>
      </div>
    )
  }

  const shippingFee = subtotal() >= 999 ? 0 : 60
  const total = subtotal() + shippingFee

  return (
    <div className="mx-auto max-w-5xl px-4 py-8 sm:px-6 lg:px-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-8">Shopping Cart ({items.length} items)</h1>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        {/* Items */}
        <div className="lg:col-span-2 space-y-4">
          {items.map((item) => (
            <div key={`${item.productId}_${item.variantId}`} className="card p-4 flex gap-4">
              <div className="relative h-20 w-20 flex-shrink-0 overflow-hidden rounded-lg bg-gray-100">
                {item.imageUrl ? (
                  <Image src={item.imageUrl} alt={item.productName} fill className="object-cover" sizes="80px" />
                ) : (
                  <div className="flex h-full items-center justify-center text-gray-300"><ShoppingBag size={24} /></div>
                )}
              </div>

              <div className="flex-1 min-w-0">
                <Link href={`/products/${item.productSlug}`} className="font-medium text-gray-900 hover:text-primary-600 transition-colors line-clamp-2">
                  {item.productName}
                </Link>
                {item.variantAttributes && (
                  <p className="text-xs text-gray-500 mt-0.5">
                    {Object.entries(item.variantAttributes).map(([k, v]) => `${k}: ${v}`).join(', ')}
                  </p>
                )}
                <p className="text-sm font-bold text-primary-700 mt-1">{formatBDT(item.price)}</p>

                <div className="mt-2 flex items-center gap-3">
                  <div className="flex items-center rounded-lg border">
                    <button onClick={() => updateQuantity(item.productId, item.variantId, item.quantity - 1)} className="px-2.5 py-1 text-gray-500 hover:bg-gray-50">−</button>
                    <span className="px-3 py-1 text-sm font-semibold">{item.quantity}</span>
                    <button onClick={() => updateQuantity(item.productId, item.variantId, item.quantity + 1)} className="px-2.5 py-1 text-gray-500 hover:bg-gray-50">+</button>
                  </div>
                  <button onClick={() => removeItem(item.productId, item.variantId)} className="rounded p-1 text-red-500 hover:bg-red-50 transition-colors">
                    <Trash2 size={16} />
                  </button>
                </div>
              </div>

              <div className="flex-shrink-0 text-right">
                <p className="font-bold text-gray-900">{formatBDT(item.price * item.quantity)}</p>
              </div>
            </div>
          ))}
        </div>

        {/* Summary */}
        <div className="card p-5 h-fit space-y-4">
          <h2 className="text-base font-bold text-gray-900">Order Summary</h2>
          <div className="space-y-2 text-sm">
            <div className="flex justify-between text-gray-600">
              <span>Subtotal</span><span>{formatBDT(subtotal())}</span>
            </div>
            <div className="flex justify-between text-gray-600">
              <span>Shipping</span>
              <span>{shippingFee === 0 ? <span className="text-green-600 font-medium">Free</span> : formatBDT(shippingFee)}</span>
            </div>
            {shippingFee > 0 && (
              <p className="text-xs text-gray-400">Add {formatBDT(999 - subtotal())} more for free shipping</p>
            )}
            <hr />
            <div className="flex justify-between font-bold text-gray-900 text-base">
              <span>Total</span><span>{formatBDT(total)}</span>
            </div>
          </div>
          <Link href="/checkout" className="btn-primary w-full text-center block">
            Proceed to Checkout
          </Link>
          <Link href="/products" className="btn-outline w-full text-center block">
            Continue Shopping
          </Link>
        </div>
      </div>
    </div>
  )
}
