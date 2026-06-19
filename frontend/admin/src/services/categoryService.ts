import { api } from '@/lib/api'
import type { ApiResponse, CategoryDto } from '@/types'

export interface CreateCategoryRequest {
  name: string
  slug: string
  parentId?: string | null
  description?: string
  imageUrl?: string
  sortOrder?: number
}

export const categoryService = {
  // Returns the tree of root categories (each has .children[])
  getCategories: () =>
    api
      .get<ApiResponse<CategoryDto[]>>('/categories')
      .then((r) => r.data.data ?? []),

  getCategoryById: (id: string) =>
    api
      .get<ApiResponse<CategoryDto>>(`/categories/${id}`)
      .then((r) => r.data.data),

  createCategory: (data: CreateCategoryRequest) =>
    api
      .post<ApiResponse<CategoryDto>>('/categories', data)
      .then((r) => r.data.data),

  updateCategory: (id: string, data: Partial<CreateCategoryRequest>) =>
    api
      .put<ApiResponse<CategoryDto>>(`/categories/${id}`, data)
      .then((r) => r.data.data),

  deleteCategory: (id: string) =>
    api.delete<ApiResponse<null>>(`/categories/${id}`).then((r) => r.data),
}
