import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { reviewService } from '@/services/reviewService'
import Badge from '@/components/ui/Badge'
import { Star, CheckCircle, XCircle } from 'lucide-react'
import { formatDate } from '@/lib/utils'

function Stars({ rating }: { rating: number }) {
  return (
    <div className="flex gap-0.5">
      {Array.from({ length: 5 }).map((_, i) => (
        <Star key={i} size={12} className={i < rating ? 'text-yellow-400 fill-yellow-400' : 'text-gray-200 fill-gray-200'} />
      ))}
    </div>
  )
}

export default function ReviewsPage() {
  const qc = useQueryClient()
  const { data, isLoading } = useQuery({
    queryKey: ['reviews'],
    queryFn: () => reviewService.getReviews({ status: 'Pending' }),
  })

  const reviews = data?.items ?? []

  const { mutate: approve } = useMutation({
    mutationFn: reviewService.approveReview,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['reviews'] }),
  })

  const { mutate: reject } = useMutation({
    mutationFn: reviewService.rejectReview,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['reviews'] }),
  })

  return (
    <div className="bg-white rounded-xl border overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-gray-50">
              <th className="px-4 py-3 text-left font-medium text-gray-500">Customer</th>
              <th className="px-4 py-3 text-left font-medium text-gray-500">Rating</th>
              <th className="px-4 py-3 text-left font-medium text-gray-500">Review</th>
              <th className="px-4 py-3 text-center font-medium text-gray-500">Verified</th>
              <th className="px-4 py-3 text-center font-medium text-gray-500">Status</th>
              <th className="px-4 py-3 text-left font-medium text-gray-500">Date</th>
              <th className="px-4 py-3 text-center font-medium text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {isLoading && Array.from({ length: 5 }).map((_, i) => (
              <tr key={i}>{Array.from({ length: 7 }).map((_, j) => (
                <td key={j} className="px-4 py-3"><div className="h-4 bg-gray-200 rounded animate-pulse" /></td>
              ))}</tr>
            ))}
            {!isLoading && reviews.map((review) => (
              <tr key={review.id} className="hover:bg-gray-50 transition-colors">
                <td className="px-4 py-3 text-gray-700">{review.customerName}</td>
                <td className="px-4 py-3"><Stars rating={review.rating} /></td>
                <td className="px-4 py-3 max-w-xs">
                  {review.title && <p className="font-medium text-gray-800">{review.title}</p>}
                  <p className="text-gray-500 truncate">{review.body ?? '—'}</p>
                </td>
                <td className="px-4 py-3 text-center">
                  <Badge variant={review.isVerifiedPurchase ? 'success' : 'default'}>
                    {review.isVerifiedPurchase ? 'Verified' : 'Unverified'}
                  </Badge>
                </td>
                <td className="px-4 py-3 text-center">
                  <Badge variant={review.status === 'Approved' ? 'success' : review.status === 'Rejected' ? 'danger' : 'warning'}>
                    {review.status}
                  </Badge>
                </td>
                <td className="px-4 py-3 text-gray-500">{formatDate(review.createdAt)}</td>
                <td className="px-4 py-3">
                  <div className="flex items-center justify-center gap-2">
                    {review.status !== 'Approved' && (
                      <button onClick={() => approve(review.id)} title="Approve"
                        className="rounded p-1 text-green-600 hover:bg-green-50 transition-colors">
                        <CheckCircle size={16} />
                      </button>
                    )}
                    {review.status !== 'Rejected' && (
                      <button onClick={() => reject(review.id)} title="Reject"
                        className="rounded p-1 text-red-500 hover:bg-red-50 transition-colors">
                        <XCircle size={16} />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
            {!isLoading && reviews.length === 0 && (
              <tr><td colSpan={7} className="px-4 py-10 text-center text-gray-400">No reviews pending</td></tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
