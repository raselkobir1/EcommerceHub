import { api } from '@/lib/api'
import type { ApiResponse, BannerDto, BannerPosition, PagedResult } from '@/types'

export interface CreateBannerRequest {
  title: string
  imageUrl: string
  linkUrl?: string
  position: BannerPosition
  sortOrder?: number
  isActive?: boolean
  startsAt?: string
  expiresAt?: string
}

export const bannerService = {
  getBanners: (params?: { page?: number; pageSize?: number; position?: BannerPosition }) =>
    api
      .get<ApiResponse<PagedResult<BannerDto>>>('/promotions/banners', { params })
      .then((r) => r.data.data),

  createBanner: (data: CreateBannerRequest) =>
    api
      .post<ApiResponse<BannerDto>>('/promotions/banners', data)
      .then((r) => r.data.data),

  updateBanner: (id: string, data: Partial<CreateBannerRequest>) =>
    api
      .put<ApiResponse<BannerDto>>(`/promotions/banners/${id}`, data)
      .then((r) => r.data.data),

  deleteBanner: (id: string) =>
    api.delete<ApiResponse<null>>(`/promotions/banners/${id}`).then((r) => r.data),
}
