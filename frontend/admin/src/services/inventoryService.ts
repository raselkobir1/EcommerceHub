import { api } from '@/lib/api'
import type {
  AdjustmentReason,
  ApiResponse,
  InventoryItemDto,
  PagedResult,
  StockAdjustmentDto,
} from '@/types'

export interface AdjustStockRequest {
  variantId: string
  quantityChange: number
  reason: AdjustmentReason
  notes?: string
}

export const inventoryService = {
  getInventory: (params?: {
    page?: number
    pageSize?: number
    search?: string
    lowStock?: boolean
  }) =>
    api
      .get<ApiResponse<PagedResult<InventoryItemDto>>>('/inventory/items', { params })
      .then((r) => r.data.data),

  adjustStock: (data: AdjustStockRequest) =>
    api
      .post<ApiResponse<StockAdjustmentDto>>('/inventory/adjustments', data)
      .then((r) => r.data.data),

  getAdjustmentHistory: (productId: string) =>
    api
      .get<ApiResponse<PagedResult<StockAdjustmentDto>>>(`/inventory/adjustments/product/${productId}`)
      .then((r) => r.data.data),
}
