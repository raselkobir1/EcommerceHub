import { api } from '@/lib/api'
import type { ApiResponse, PagedResult, ReviewDto, ReviewStatus } from '@/types'

export const reviewService = {
  getReviews: (params?: {
    page?: number
    pageSize?: number
    status?: ReviewStatus
    productId?: string
  }) =>
    api
      .get<ApiResponse<PagedResult<ReviewDto>>>('/reviews', { params })
      .then((r) => r.data.data),

  approveReview: (id: string) =>
    api
      .post<ApiResponse<ReviewDto>>(`/reviews/${id}/approve`)
      .then((r) => r.data.data),

  rejectReview: (id: string) =>
    api
      .post<ApiResponse<ReviewDto>>(`/reviews/${id}/reject`)
      .then((r) => r.data.data),

  deleteReview: (id: string) =>
    api.delete<ApiResponse<null>>(`/reviews/${id}`).then((r) => r.data),
}
