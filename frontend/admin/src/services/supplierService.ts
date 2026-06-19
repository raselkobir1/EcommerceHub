import { api } from '@/lib/api'
import type { ApiResponse, PagedResult, SupplierDto } from '@/types'

export interface CreateSupplierRequest {
  name: string
  contactPerson?: string
  email?: string
  phone?: string
  address?: string
  taxId?: string
  paymentTerms?: string
}

export const supplierService = {
  getSuppliers: (params?: { page?: number; pageSize?: number; search?: string }) =>
    api
      .get<ApiResponse<PagedResult<SupplierDto>>>('/suppliers', { params })
      .then((r) => r.data.data),

  getSupplierById: (id: string) =>
    api
      .get<ApiResponse<SupplierDto>>(`/suppliers/${id}`)
      .then((r) => r.data.data),

  createSupplier: (data: CreateSupplierRequest) =>
    api
      .post<ApiResponse<SupplierDto>>('/suppliers', data)
      .then((r) => r.data.data),

  updateSupplier: (id: string, data: Partial<CreateSupplierRequest>) =>
    api
      .put<ApiResponse<SupplierDto>>(`/suppliers/${id}`, data)
      .then((r) => r.data.data),

  deleteSupplier: (id: string) =>
    api.delete<ApiResponse<null>>(`/suppliers/${id}`).then((r) => r.data),
}
