import { api } from '@/lib/api'
import type { ApiResponse, CouponDto, DiscountType, PagedResult } from '@/types'

export interface CreateCouponRequest {
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  isActive?: boolean
  startsAt: string
  expiresAt?: string
}

export const couponService = {
  getCoupons: (params?: { page?: number; pageSize?: number; search?: string }) =>
    api
      .get<ApiResponse<PagedResult<CouponDto>>>('/promotions/coupons', { params })
      .then((r) => r.data.data),

  createCoupon: (data: CreateCouponRequest) =>
    api
      .post<ApiResponse<CouponDto>>('/promotions/coupons', data)
      .then((r) => r.data.data),

  updateCoupon: (id: string, data: Partial<CreateCouponRequest>) =>
    api
      .put<ApiResponse<CouponDto>>(`/promotions/coupons/${id}`, data)
      .then((r) => r.data.data),

  toggleCoupon: (id: string, isActive: boolean) =>
    api
      .patch<ApiResponse<CouponDto>>(`/promotions/coupons/${id}/toggle`, { isActive })
      .then((r) => r.data.data),

  deleteCoupon: (id: string) =>
    api.delete<ApiResponse<null>>(`/promotions/coupons/${id}`).then((r) => r.data),
}
