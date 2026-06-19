'use client'
import { useState } from 'react'
import Image from 'next/image'
import { useParams } from 'next/navigation'
import { useQuery } from '@tanstack/react-query'
import { ShoppingCart, CheckCircle } from 'lucide-react'
import { productService } from '@/services/productService'
import { useCartStore } from '@/stores/cartStore'
import BdtPrice from '@/components/ui/BdtPrice'
import StarRating from '@/components/ui/StarRating'

export default function ProductDetailPage() {
  const { slug } = useParams<{ slug: string }>()
  const addItem = useCartStore((s) => s.addItem)
  const [selectedImage, setSelectedImage] = useState(0)
  const [selectedVariantId, setSelectedVariantId] = useState<string | undefined>()
  const [qty, setQty] = useState(1)
  const [added, setAdded] = useState(false)

  const { data: product, isLoading } = useQuery({
    queryKey: ['product', slug],
    queryFn: () => productService.getProductBySlug(slug),
    enabled: !!slug,
  })

  const { data: reviews } = useQuery({
    queryKey: ['product-reviews', product?.id],
    queryFn: () => productService.getProductReviews(product!.id, { pageSize: 5 }),
    enabled: !!product?.id,
  })

  if (isLoading) {
    return (
      <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 gap-8 lg:grid-cols-2 animate-pulse">
          <div className="aspect-square rounded-2xl bg-gray-200" />
          <div className="space-y-4">
            <div className="h-8 bg-gray-200 rounded w-3/4" />
            <div className="h-6 bg-gray-200 rounded w-1/2" />
            <div className="h-10 bg-gray-200 rounded w-1/3" />
          </div>
        </div>
      </div>
    )
  }

  if (!product) return <div className="py-24 text-center text-gray-400">Product not found</div>

  const images = product.images ?? []
  const variants = product.variants ?? []

  const selectedVariant = variants.find((v) => v.id === selectedVariantId)
  const price = selectedVariant?.salePrice ?? selectedVariant?.price ?? product.salePrice ?? product.price
  const originalPrice = selectedVariant?.price ?? product.price
  const inStock = (selectedVariant?.stockQuantity ?? product.stockQuantity) > 0

  const handleAddToCart = () => {
    addItem({
      productId: product.id,
      productName: product.name,
      productSlug: product.slug,
      imageUrl: images[selectedImage]?.url,
      variantId: selectedVariantId,
      variantAttributes: selectedVariant?.attributes,
      price,
      quantity: qty,
    })
    setAdded(true)
    setTimeout(() => setAdded(false), 2000)
  }

  const variantGroups = variants.reduce<Record<string, string[]>>((acc, v) => {
    Object.entries(v.attributes).forEach(([k, val]) => {
      if (!acc[k]) acc[k] = []
      if (!acc[k].includes(val)) acc[k].push(val)
    })
    return acc
  }, {})

  return (
    <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8 space-y-12">
      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        {/* Images */}
        <div className="space-y-3">
          <div className="relative aspect-square overflow-hidden rounded-2xl bg-gray-100">
            {images[selectedImage] ? (
              <Image
                src={images[selectedImage].url}
                alt={images[selectedImage].altText ?? product.name}
                fill
                className="object-cover"
                priority
                sizes="(max-width: 1024px) 100vw, 50vw"
              />
            ) : (
              <div className="flex h-full items-center justify-center text-gray-300">
                <ShoppingCart size={64} />
              </div>
            )}
          </div>
          {images.length > 1 && (
            <div className="flex gap-2 overflow-x-auto pb-1">
              {images.map((img, i) => (
                <button key={img.id} onClick={() => setSelectedImage(i)}
                  className={`relative h-16 w-16 flex-shrink-0 overflow-hidden rounded-lg border-2 transition-colors ${i === selectedImage ? 'border-primary-500' : 'border-gray-200 hover:border-gray-400'}`}>
                  <Image src={img.url} alt={img.altText ?? ''} fill className="object-cover" sizes="64px" />
                </button>
              ))}
            </div>
          )}
        </div>

        {/* Info */}
        <div className="space-y-5">
          {product.brandName && <p className="text-sm font-medium text-primary-600">{product.brandName}</p>}
          <h1 className="text-2xl font-bold text-gray-900 lg:text-3xl">{product.name}</h1>

          {product.averageRating !== undefined && (
            <StarRating rating={product.averageRating} count={product.reviewCount} size={18} />
          )}

          <BdtPrice price={originalPrice} salePrice={price < originalPrice ? price : undefined} size="lg" />

          {product.shortDescription && (
            <p className="text-gray-600 leading-relaxed">{product.shortDescription}</p>
          )}

          {/* Variants */}
          {Object.entries(variantGroups).map(([key, values]) => (
            <div key={key}>
              <p className="text-sm font-medium text-gray-700 mb-2">{key}</p>
              <div className="flex flex-wrap gap-2">
                {values.map((val) => {
                  const matchingVariant = variants.find((v) => v.attributes[key] === val)
                  const isSelected = selectedVariantId === matchingVariant?.id
                  return (
                    <button key={val} onClick={() => setSelectedVariantId(matchingVariant?.id)}
                      className={`rounded-lg border px-4 py-2 text-sm font-medium transition-colors ${isSelected ? 'border-primary-500 bg-primary-50 text-primary-700' : 'border-gray-300 text-gray-700 hover:border-gray-400'}`}>
                      {val}
                    </button>
                  )
                })}
              </div>
            </div>
          ))}

          {/* Qty + Cart */}
          <div className="flex items-center gap-3">
            <div className="flex items-center rounded-lg border">
              <button onClick={() => setQty(Math.max(1, qty - 1))} className="px-3 py-2 text-gray-500 hover:bg-gray-50 transition-colors">−</button>
              <span className="px-4 py-2 text-sm font-semibold min-w-[3rem] text-center">{qty}</span>
              <button onClick={() => setQty(qty + 1)} className="px-3 py-2 text-gray-500 hover:bg-gray-50 transition-colors">+</button>
            </div>
            <button onClick={handleAddToCart} disabled={!inStock}
              className="flex flex-1 items-center justify-center gap-2 rounded-xl bg-primary-600 px-6 py-3 text-sm font-bold text-white hover:bg-primary-700 transition-colors disabled:opacity-40 disabled:cursor-not-allowed">
              {added ? <><CheckCircle size={16} /> Added!</> : <><ShoppingCart size={16} /> Add to Cart</>}
            </button>
          </div>

          {!inStock && <p className="text-sm font-medium text-red-500">Out of stock</p>}

          <div className="rounded-xl bg-gray-50 p-4 text-sm text-gray-600 space-y-1">
            <p>✓ Free delivery on orders over ৳999</p>
            <p>✓ 7-day easy return policy</p>
            <p>✓ Genuine product guaranteed</p>
          </div>
        </div>
      </div>

      {/* Description */}
      {product.fullDescription && (
        <div className="card p-6">
          <h2 className="text-lg font-bold text-gray-900 mb-4">Product Description</h2>
          <div className="prose prose-sm max-w-none text-gray-600 whitespace-pre-wrap">{product.fullDescription}</div>
        </div>
      )}

      {/* Reviews */}
      {reviews && reviews.items.length > 0 && (
        <div className="card p-6">
          <h2 className="text-lg font-bold text-gray-900 mb-6">Customer Reviews</h2>
          <div className="space-y-5">
            {reviews.items.map((review) => (
              <div key={review.id} className="border-b last:border-0 pb-5 last:pb-0">
                <div className="flex items-start justify-between gap-4">
                  <div>
                    <p className="font-medium text-gray-900">{review.customerName}</p>
                    <StarRating rating={review.rating} size={14} className="mt-0.5" />
                  </div>
                  {review.isVerifiedPurchase && (
                    <span className="flex-shrink-0 rounded-full bg-green-50 px-2 py-0.5 text-xs font-medium text-green-700">Verified Purchase</span>
                  )}
                </div>
                {review.title && <p className="mt-2 font-semibold text-gray-800">{review.title}</p>}
                {review.body && <p className="mt-1 text-sm text-gray-600">{review.body}</p>}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
