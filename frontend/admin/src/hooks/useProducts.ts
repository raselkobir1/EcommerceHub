import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { productService } from '@/services/productService'

export const useProducts = (params?: Parameters<typeof productService.getProducts>[0]) =>
  useQuery({ queryKey: ['products', params], queryFn: () => productService.getProducts(params) })

export const useCreateProduct = () => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: productService.createProduct,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }),
  })
}
