import { api } from '@/lib/api'
import type { ApiResponse, PagedResult, ProductDetailDto, ProductListDto } from '@/types'

export interface ProductQueryParams {
  page?: number
  pageSize?: number
  search?: string
  status?: string
  categoryId?: string
  brandId?: string
  isFeatured?: boolean
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

export interface CreateProductRequest {
  name: string
  slug: string
  shortDescription?: string
  fullDescription?: string
  categoryId: string
  brandId?: string
  basePrice: number
  compareAtPrice?: number
  metaTitle?: string
  metaDescription?: string
  tags?: string[]
  isFeatured?: boolean
  variants: CreateVariantRequest[]
}

export interface CreateVariantRequest {
  sku: string
  price: number
  compareAtPrice?: number
  stockQuantity: number
  attributes?: Record<string, string>
}

export interface UpdateProductRequest extends Partial<CreateProductRequest> {
  status?: string
}

export const productService = {
  getProducts: (params: ProductQueryParams = {}) =>
    api
      .get<ApiResponse<PagedResult<ProductListDto>>>('/products', { params })
      .then((r) => r.data.data),

  getProductById: (id: string) =>
    api
      .get<ApiResponse<ProductDetailDto>>(`/products/${id}`)
      .then((r) => r.data.data),

  createProduct: (data: CreateProductRequest) =>
    api
      .post<ApiResponse<ProductDetailDto>>('/products', data)
      .then((r) => r.data.data),

  updateProduct: (id: string, data: UpdateProductRequest) =>
    api
      .put<ApiResponse<ProductDetailDto>>(`/products/${id}`, data)
      .then((r) => r.data.data),

  deleteProduct: (id: string) =>
    api.delete<ApiResponse<null>>(`/products/${id}`).then((r) => r.data),

  publishProduct: (id: string) =>
    api
      .post<ApiResponse<ProductDetailDto>>(`/products/${id}/publish`)
      .then((r) => r.data.data),

  archiveProduct: (id: string) =>
    api
      .post<ApiResponse<ProductDetailDto>>(`/products/${id}/archive`)
      .then((r) => r.data.data),
}
