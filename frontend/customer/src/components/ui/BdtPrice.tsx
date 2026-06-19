import { cn } from '@/lib/utils'

interface BdtPriceProps {
  price: number
  salePrice?: number
  className?: string
  size?: 'sm' | 'md' | 'lg'
}

export default function BdtPrice({ price, salePrice, className, size = 'md' }: BdtPriceProps) {
  const hasDiscount = salePrice !== undefined && salePrice < price
  const display = hasDiscount ? salePrice : price
  const discountPct = hasDiscount ? Math.round(((price - salePrice) / price) * 100) : 0

  const sizeClass = { sm: 'text-sm', md: 'text-base', lg: 'text-xl' }[size]
  const crossClass = { sm: 'text-xs', md: 'text-sm', lg: 'text-base' }[size]

  return (
    <span className={cn('inline-flex items-baseline gap-2', className)}>
      <span className={cn('font-bold text-primary-700', sizeClass)}>
        ৳{display.toLocaleString('en-BD')}
      </span>
      {hasDiscount && (
        <>
          <span className={cn('line-through text-gray-400', crossClass)}>৳{price.toLocaleString('en-BD')}</span>
          <span className="rounded bg-red-100 px-1.5 py-0.5 text-xs font-semibold text-red-600">-{discountPct}%</span>
        </>
      )}
    </span>
  )
}
