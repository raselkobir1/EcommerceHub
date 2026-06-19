'use client'
import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { CartItem } from '@/types'

interface CartState {
  items: CartItem[]
  couponCode: string | null
  addItem: (item: CartItem) => void
  removeItem: (productId: string, variantId?: string) => void
  updateQuantity: (productId: string, variantId: string | undefined, qty: number) => void
  clearCart: () => void
  setCoupon: (code: string | null) => void
  itemCount: () => number
  subtotal: () => number
}

const key = (productId: string, variantId?: string) => `${productId}_${variantId ?? 'default'}`

export const useCartStore = create<CartState>()(
  persist(
    (set, get) => ({
      items: [],
      couponCode: null,
      addItem: (item) => {
        const k = key(item.productId, item.variantId)
        set((s) => {
          const existing = s.items.find((i) => key(i.productId, i.variantId) === k)
          if (existing) {
            return { items: s.items.map((i) => key(i.productId, i.variantId) === k ? { ...i, quantity: i.quantity + item.quantity } : i) }
          }
          return { items: [...s.items, item] }
        })
      },
      removeItem: (productId, variantId) => {
        const k = key(productId, variantId)
        set((s) => ({ items: s.items.filter((i) => key(i.productId, i.variantId) !== k) }))
      },
      updateQuantity: (productId, variantId, qty) => {
        const k = key(productId, variantId)
        if (qty <= 0) {
          set((s) => ({ items: s.items.filter((i) => key(i.productId, i.variantId) !== k) }))
        } else {
          set((s) => ({ items: s.items.map((i) => key(i.productId, i.variantId) === k ? { ...i, quantity: qty } : i) }))
        }
      },
      clearCart: () => set({ items: [], couponCode: null }),
      setCoupon: (code) => set({ couponCode: code }),
      itemCount: () => get().items.reduce((acc, i) => acc + i.quantity, 0),
      subtotal: () => get().items.reduce((acc, i) => acc + i.price * i.quantity, 0),
    }),
    { name: 'customer-cart' }
  )
)
