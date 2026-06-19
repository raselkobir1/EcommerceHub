'use client'
import Link from 'next/link'
import Image from 'next/image'
import { ShoppingCart } from 'lucide-react'
import { useCartStore } from '@/stores/cartStore'
import BdtPrice from '@/components/ui/BdtPrice'
import StarRating from '@/components/ui/StarRating'
import type { Product } from '@/types'

interface ProductCardProps {
  product: Product
}

export default function ProductCard({ product }: ProductCardProps) {
  const addItem = useCartStore((s) => s.addItem)
  const primaryImage = product.images.find((i) => i.isPrimary) ?? product.images[0]
  const inStock = product.stockQuantity > 0

  const handleAddToCart = (e: React.MouseEvent) => {
    e.preventDefault()
    if (!inStock) return
    addItem({
      productId: product.id,
      productName: product.name,
      productSlug: product.slug,
      imageUrl: primaryImage?.url,
      price: product.salePrice ?? product.price,
      quantity: 1,
    })
  }

  return (
    <Link href={`/products/${product.slug}`} className="group block">
      <div className="card overflow-hidden transition-shadow hover:shadow-md">
        <div className="relative aspect-square overflow-hidden bg-gray-100">
          {primaryImage ? (
            <Image
              src={primaryImage.url}
              alt={primaryImage.altText ?? product.name}
              fill
              className="object-cover group-hover:scale-105 transition-transform duration-300"
              sizes="(max-width: 640px) 50vw, (max-width: 1024px) 33vw, 25vw"
            />
          ) : (
            <div className="flex h-full items-center justify-center text-gray-300">
              <ShoppingCart size={40} />
            </div>
          )}
          {product.salePrice && product.salePrice < product.price && (
            <span className="absolute top-2 left-2 rounded bg-red-500 px-2 py-0.5 text-xs font-bold text-white">
              SALE
            </span>
          )}
          {!inStock && (
            <div className="absolute inset-0 flex items-center justify-center bg-black/40">
              <span className="rounded bg-white px-3 py-1 text-sm font-semibold text-gray-800">Out of Stock</span>
            </div>
          )}
        </div>

        <div className="p-3">
          <p className="text-xs text-gray-400 mb-0.5">{product.brandName}</p>
          <h3 className="text-sm font-medium text-gray-900 line-clamp-2 group-hover:text-primary-600 transition-colors">
            {product.name}
          </h3>

          {product.averageRating !== undefined && (
            <div className="mt-1">
              <StarRating rating={product.averageRating} count={product.reviewCount} size={12} />
            </div>
          )}

          <div className="mt-2 flex items-center justify-between gap-2">
            <BdtPrice price={product.price} salePrice={product.salePrice} size="sm" />
            <button
              onClick={handleAddToCart}
              disabled={!inStock}
              className="rounded-lg bg-primary-600 p-2 text-white hover:bg-primary-700 transition-colors disabled:opacity-40 disabled:cursor-not-allowed"
              title="Add to cart"
            >
              <ShoppingCart size={14} />
            </button>
          </div>
        </div>
      </div>
    </Link>
  )
}
