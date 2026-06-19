import { useQuery } from '@tanstack/react-query'
import { orderService } from '@/services/orderService'

export const useOrders = (params?: Parameters<typeof orderService.getOrders>[0]) =>
  useQuery({ queryKey: ['orders', params], queryFn: () => orderService.getOrders(params) })
