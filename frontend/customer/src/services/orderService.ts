import api from '@/lib/api'
import type { ApiResponse, PagedResult, Order } from '@/types'

interface PlaceOrderPayload {
  items: { productId: string; variantId?: string; quantity: number }[]
  shippingAddress: {
    recipientName: string
    phone: string
    addressLine1: string
    addressLine2?: string
    areaThana: string
    district: string
    division: string
  }
  paymentMethod: string
  couponCode?: string
}

export const orderService = {
  placeOrder: async (payload: PlaceOrderPayload) => {
    const { data } = await api.post<ApiResponse<{ orderId: string; orderNumber: string }>>('/api/orders', payload)
    return data.data
  },

  getMyOrders: async (params?: { page?: number; pageSize?: number; status?: string }) => {
    const { data } = await api.get<ApiResponse<PagedResult<Order>>>('/api/orders/my', { params })
    return data.data
  },

  getOrderByNumber: async (orderNumber: string) => {
    const { data } = await api.get<ApiResponse<Order>>(`/api/orders/my/${orderNumber}`)
    return data.data
  },
}
