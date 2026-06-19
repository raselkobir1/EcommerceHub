import { useQuery } from '@tanstack/react-query'
import { couponService } from '@/services/couponService'
import Badge from '@/components/ui/Badge'
import { formatDate, formatBDT } from '@/lib/utils'

export default function CouponsPage() {
  const { data, isLoading } = useQuery({
    queryKey: ['coupons'],
    queryFn: () => couponService.getCoupons(),
  })

  const coupons = data?.items ?? []

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <button className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 transition-colors">
          + Create Coupon
        </button>
      </div>

      <div className="bg-white rounded-xl border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-gray-50">
                <th className="px-4 py-3 text-left font-medium text-gray-500">Code</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Type</th>
                <th className="px-4 py-3 text-right font-medium text-gray-500">Value</th>
                <th className="px-4 py-3 text-center font-medium text-gray-500">Usage</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Expiry</th>
                <th className="px-4 py-3 text-center font-medium text-gray-500">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {isLoading && Array.from({ length: 5 }).map((_, i) => (
                <tr key={i}>{Array.from({ length: 6 }).map((_, j) => (
                  <td key={j} className="px-4 py-3"><div className="h-4 bg-gray-200 rounded animate-pulse" /></td>
                ))}</tr>
              ))}
              {!isLoading && coupons.map((coupon) => (
                <tr key={coupon.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-mono font-semibold text-primary-700">{coupon.code}</td>
                  <td className="px-4 py-3 text-gray-600">{coupon.discountType}</td>
                  <td className="px-4 py-3 text-right font-medium">
                    {coupon.discountType === 'Percentage' ? `${coupon.discountValue}%` : formatBDT(coupon.discountValue)}
                  </td>
                  <td className="px-4 py-3 text-center text-gray-600">
                    {coupon.usageCount}{coupon.usageLimit ? ` / ${coupon.usageLimit}` : ''}
                  </td>
                  <td className="px-4 py-3 text-gray-500">{coupon.expiresAt ? formatDate(coupon.expiresAt) : 'No expiry'}</td>
                  <td className="px-4 py-3 text-center">
                    <Badge variant={coupon.isActive ? 'success' : 'default'}>
                      {coupon.isActive ? 'Active' : 'Inactive'}
                    </Badge>
                  </td>
                </tr>
              ))}
              {!isLoading && coupons.length === 0 && (
                <tr><td colSpan={6} className="px-4 py-10 text-center text-gray-400">No coupons found</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
