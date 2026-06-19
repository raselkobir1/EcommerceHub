import { api } from '@/lib/api'
import type { ApiResponse, OrderDto, OrderStatus, OrderSummaryDto, PagedResult } from '@/types'

export interface OrderQueryParams {
  page?: number
  pageSize?: number
  status?: OrderStatus | ''
  paymentStatus?: string
  search?: string
  fromDate?: string
  toDate?: string
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

export const orderService = {
  getOrders: (params: OrderQueryParams = {}) =>
    api
      .get<ApiResponse<PagedResult<OrderSummaryDto>>>('/orders', { params })
      .then((r) => r.data.data),

  getOrderById: (id: string) =>
    api
      .get<ApiResponse<OrderDto>>(`/orders/${id}`)
      .then((r) => r.data.data),

  updateOrderStatus: (id: string, status: OrderStatus, note?: string) =>
    api
      .put<ApiResponse<OrderDto>>(`/orders/${id}/status`, { status, note })
      .then((r) => r.data.data),

  cancelOrder: (id: string, reason: string) =>
    api
      .put<ApiResponse<OrderDto>>(`/orders/${id}/cancel`, { reason })
      .then((r) => r.data.data),
}
