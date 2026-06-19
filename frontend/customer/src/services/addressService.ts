import api from '@/lib/api'
import type { ApiResponse, CustomerAddress } from '@/types'

interface AddressPayload {
  label: string
  recipientName: string
  phone: string
  addressLine1: string
  addressLine2?: string
  areaThana: string
  district: string
  division: string
  isDefault?: boolean
}

export const addressService = {
  getAddresses: async () => {
    const { data } = await api.get<ApiResponse<CustomerAddress[]>>('/api/customers/addresses')
    return data.data
  },

  addAddress: async (payload: AddressPayload) => {
    const { data } = await api.post<ApiResponse<CustomerAddress>>('/api/customers/addresses', payload)
    return data.data
  },

  updateAddress: async (id: string, payload: AddressPayload) => {
    const { data } = await api.put<ApiResponse<CustomerAddress>>(`/api/customers/addresses/${id}`, payload)
    return data.data
  },

  deleteAddress: async (id: string) => {
    await api.delete(`/api/customers/addresses/${id}`)
  },

  setDefault: async (id: string) => {
    await api.patch(`/api/customers/addresses/${id}/default`)
  },
}
