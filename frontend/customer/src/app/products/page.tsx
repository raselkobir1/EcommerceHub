'use client'
import { Suspense, useState, useCallback } from 'react'
import { useSearchParams, useRouter } from 'next/navigation'
import { useQuery } from '@tanstack/react-query'
import { SlidersHorizontal, X } from 'lucide-react'
import { productService } from '@/services/productService'
import ProductGrid from '@/components/product/ProductGrid'
import { cn } from '@/lib/utils'

const SORT_OPTIONS = [
  { value: 'newest', label: 'Newest First' },
  { value: 'price_asc', label: 'Price: Low to High' },
  { value: 'price_desc', label: 'Price: High to Low' },
  { value: 'popular', label: 'Most Popular' },
  { value: 'discount', label: 'Biggest Discount' },
]

function ProductsPageContent() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const [page, setPage] = useState(1)
  const [filtersOpen, setFiltersOpen] = useState(false)

  const search = searchParams.get('search') ?? undefined
  const categorySlug = searchParams.get('category') ?? undefined
  const sort = searchParams.get('sort') ?? 'newest'
  const [minPrice, setMinPrice] = useState('')
  const [maxPrice, setMaxPrice] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['products', page, search, categorySlug, sort, minPrice, maxPrice],
    queryFn: () => productService.getProducts({
      page,
      pageSize: 16,
      search,
      categorySlug,
      sort,
      minPrice: minPrice ? Number(minPrice) : undefined,
      maxPrice: maxPrice ? Number(maxPrice) : undefined,
    }),
  })

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: () => productService.getCategories(),
  })

  const updateParam = useCallback((key: string, value: string | undefined) => {
    const params = new URLSearchParams(searchParams.toString())
    if (value) params.set(key, value)
    else params.delete(key)
    router.push(`/products?${params.toString()}`)
    setPage(1)
  }, [searchParams, router])

  const totalPages = data ? Math.ceil(data.totalCount / 16) : 0

  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex flex-col gap-6 lg:flex-row">
        {/* Sidebar */}
        <aside className={cn('lg:w-56 flex-shrink-0', filtersOpen ? 'block' : 'hidden lg:block')}>
          <div className="card p-4 space-y-5">
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-3">Categories</h3>
              <div className="space-y-1">
                <button onClick={() => updateParam('category', undefined)}
                  className={cn('block w-full text-left px-2 py-1.5 rounded text-sm', !categorySlug ? 'bg-primary-50 text-primary-700 font-medium' : 'text-gray-600 hover:bg-gray-50')}>
                  All Categories
                </button>
                {(categories ?? []).map((cat) => (
                  <button key={cat.id} onClick={() => updateParam('category', cat.slug)}
                    className={cn('block w-full text-left px-2 py-1.5 rounded text-sm', categorySlug === cat.slug ? 'bg-primary-50 text-primary-700 font-medium' : 'text-gray-600 hover:bg-gray-50')}>
                    {cat.name}
                  </button>
                ))}
              </div>
            </div>

            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-3">Price Range (৳)</h3>
              <div className="flex gap-2">
                <input value={minPrice} onChange={(e) => setMinPrice(e.target.value)} placeholder="Min" className="input text-xs" />
                <input value={maxPrice} onChange={(e) => setMaxPrice(e.target.value)} placeholder="Max" className="input text-xs" />
              </div>
              <button onClick={() => setPage(1)} className="mt-2 w-full rounded-lg bg-gray-100 py-1.5 text-xs font-medium text-gray-700 hover:bg-gray-200 transition-colors">
                Apply
              </button>
            </div>
          </div>
        </aside>

        {/* Main */}
        <div className="flex-1 min-w-0">
          {/* Toolbar */}
          <div className="flex flex-wrap items-center justify-between gap-3 mb-6">
            <div className="flex items-center gap-2">
              <button onClick={() => setFiltersOpen(!filtersOpen)} className="flex items-center gap-1.5 rounded-lg border px-3 py-2 text-sm text-gray-600 hover:bg-gray-50 lg:hidden">
                <SlidersHorizontal size={14} />Filters
              </button>
              {data && (
                <p className="text-sm text-gray-500">{data.totalCount.toLocaleString()} products</p>
              )}
              {search && (
                <span className="flex items-center gap-1 rounded-full bg-primary-50 px-3 py-1 text-xs font-medium text-primary-700">
                  &ldquo;{search}&rdquo;
                  <button onClick={() => updateParam('search', undefined)}><X size={12} /></button>
                </span>
              )}
            </div>
            <select value={sort} onChange={(e) => updateParam('sort', e.target.value)}
              className="rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20">
              {SORT_OPTIONS.map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}
            </select>
          </div>

          <ProductGrid products={data?.items} isLoading={isLoading} count={16} />

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-8 flex justify-center gap-2">
              <button disabled={page <= 1} onClick={() => setPage(page - 1)}
                className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-40 hover:bg-gray-50 transition-colors">
                Previous
              </button>
              <span className="flex items-center px-4 text-sm text-gray-600">
                Page {page} of {totalPages}
              </span>
              <button disabled={page >= totalPages} onClick={() => setPage(page + 1)}
                className="rounded-lg border px-4 py-2 text-sm font-medium disabled:opacity-40 hover:bg-gray-50 transition-colors">
                Next
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default function ProductsPage() {
  return (
    <Suspense fallback={<div className="mx-auto max-w-7xl px-4 py-8"><div className="h-96 bg-gray-100 animate-pulse rounded-xl" /></div>}>
      <ProductsPageContent />
    </Suspense>
  )
}
