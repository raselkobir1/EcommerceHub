import { Star } from 'lucide-react'
import { cn } from '@/lib/utils'

interface StarRatingProps {
  rating: number
  count?: number
  size?: number
  interactive?: boolean
  onChange?: (rating: number) => void
  className?: string
}

export default function StarRating({ rating, count, size = 16, interactive = false, onChange, className }: StarRatingProps) {
  return (
    <div className={cn('inline-flex items-center gap-1', className)}>
      <div className="flex">
        {Array.from({ length: 5 }, (_, i) => {
          const filled = i < Math.round(rating)
          return (
            <button
              key={i}
              type="button"
              disabled={!interactive}
              onClick={() => onChange?.(i + 1)}
              className={cn('focus:outline-none', interactive ? 'cursor-pointer hover:scale-110 transition-transform' : 'cursor-default')}
            >
              <Star
                size={size}
                className={cn(filled ? 'text-yellow-400 fill-yellow-400' : 'text-gray-300 fill-gray-300')}
              />
            </button>
          )
        })}
      </div>
      {count !== undefined && (
        <span className="text-sm text-gray-500">({count.toLocaleString()})</span>
      )}
    </div>
  )
}
