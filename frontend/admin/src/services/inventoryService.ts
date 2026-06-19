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
  reason: AdjustmentReason
  quantityChange: number
  notes?: string
  referenceNumber?: string
}

export const inventoryService = {
  getInventory: (params?: {
    page?: number
    pageSize?: number
    search?: string
    lowStockOnly?: boolean
  }) =>
    api
      .get<ApiResponse<PagedResult<InventoryItemDto>>>('/inventory', { params })
      .then((r) => r.data.data),

  adjustStock: (data: AdjustStockRequest) =>
    api
      .post<ApiResponse<StockAdjustmentDto>>('/inventory/adjust', data)
      .then((r) => r.data.data),

  getAdjustmentHistory: (variantId: string) =>
    api
      .get<ApiResponse<StockAdjustmentDto[]>>(`/inventory/${variantId}/history`)
      .then((r) => r.data.data),
}
