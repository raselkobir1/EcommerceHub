import api from '@/lib/api'
import type { ApiResponse, PagedResult, Product, Category, Review } from '@/types'

export const productService = {
  getProducts: async (params?: { page?: number; pageSize?: number; search?: string; categorySlug?: string; minPrice?: number; maxPrice?: number; sort?: string }) => {
    const { data } = await api.get<ApiResponse<PagedResult<Product>>>('/api/products', { params })
    const result = data.data
    if (result?.items) {
      result.items = result.items.map((p) => ({ ...p, images: p.images ?? [], variants: p.variants ?? [] }))
    }
    return result
  },

  getProductBySlug: async (slug: string) => {
    const { data } = await api.get<ApiResponse<Product>>(`/api/products/${slug}`)
    const p = data.data
    if (p) return { ...p, images: p.images ?? [], variants: p.variants ?? [] }
    return p
  },

  getCategories: async () => {
    const { data } = await api.get<ApiResponse<Category[]>>('/api/categories')
    return data.data
  },

  getProductReviews: async (productId: string, params?: { page?: number; pageSize?: number }) => {
    const { data } = await api.get<ApiResponse<PagedResult<Review>>>(`/api/reviews/products/${productId}`, { params })
    return data.data
  },

  submitReview: async (payload: { productId: string; orderId: string; rating: number; title?: string; body?: string }) => {
    const { data } = await api.post<ApiResponse<void>>('/api/reviews', payload)
    return data
  },
}
