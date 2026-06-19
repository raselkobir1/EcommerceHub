import Link from 'next/link'
import Image from 'next/image'
import { ChevronRight, Truck, RotateCcw, ShieldCheck, Headphones } from 'lucide-react'
import ProductGrid from '@/components/product/ProductGrid'
import { productService } from '@/services/productService'

async function getFeaturedProducts() {
  try {
    return await productService.getProducts({ page: 1, pageSize: 8, sort: 'newest' })
  } catch {
    return null
  }
}

const FEATURES = [
  { icon: Truck, label: 'Free Delivery', desc: 'On orders over ৳999' },
  { icon: RotateCcw, label: 'Easy Returns', desc: '7-day return policy' },
  { icon: ShieldCheck, label: 'Secure Payment', desc: 'bKash, Nagad & more' },
  { icon: Headphones, label: '24/7 Support', desc: 'Always here to help' },
]

export default async function HomePage() {
  const featured = await getFeaturedProducts()

  return (
    <div className="space-y-12 pb-16">
      {/* Hero */}
      <section className="relative overflow-hidden bg-gradient-to-br from-primary-700 to-primary-900 text-white">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-20 lg:py-28">
          <div className="max-w-2xl space-y-6">
            <h1 className="text-4xl font-extrabold leading-tight sm:text-5xl lg:text-6xl">
              Shop the Best<br />Deals in Bangladesh
            </h1>
            <p className="text-lg text-primary-200">
              Millions of products. Fast delivery. Secure payment with bKash, Nagad & more.
            </p>
            <div className="flex flex-wrap gap-3">
              <Link href="/products" className="inline-flex items-center gap-2 rounded-xl bg-white px-6 py-3 text-sm font-bold text-primary-700 hover:bg-primary-50 transition-colors">
                Shop Now <ChevronRight size={16} />
              </Link>
              <Link href="/register" className="inline-flex items-center gap-2 rounded-xl border border-white/40 px-6 py-3 text-sm font-medium text-white hover:bg-white/10 transition-colors">
                Create Account
              </Link>
            </div>
          </div>
        </div>
        <div className="absolute -right-20 -bottom-10 h-72 w-72 rounded-full bg-primary-600/30 blur-3xl" />
      </section>

      {/* Features */}
      <section className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
          {FEATURES.map(({ icon: Icon, label, desc }) => (
            <div key={label} className="card flex items-start gap-3 p-4">
              <div className="rounded-lg bg-primary-50 p-2.5">
                <Icon size={20} className="text-primary-600" />
              </div>
              <div>
                <p className="text-sm font-semibold text-gray-900">{label}</p>
                <p className="text-xs text-gray-500">{desc}</p>
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* Featured Products */}
      <section className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <h2 className="text-2xl font-bold text-gray-900">New Arrivals</h2>
          <Link href="/products" className="text-sm font-medium text-primary-600 hover:underline flex items-center gap-1">
            View all <ChevronRight size={14} />
          </Link>
        </div>
        {featured ? (
          <ProductGrid products={featured.items} />
        ) : (
          <ProductGrid isLoading />
        )}
      </section>

      {/* Promo Banner */}
      <section className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="rounded-2xl bg-gradient-to-r from-orange-500 to-red-500 p-8 text-white flex flex-col sm:flex-row items-center justify-between gap-6">
          <div>
            <p className="text-sm font-semibold uppercase tracking-wider opacity-80">Limited Time Offer</p>
            <h3 className="text-3xl font-extrabold mt-1">Up to 50% OFF</h3>
            <p className="text-orange-100 mt-1">On selected electronics, fashion & home goods</p>
          </div>
          <Link href="/products?sort=discount" className="flex-shrink-0 rounded-xl bg-white px-6 py-3 text-sm font-bold text-red-600 hover:bg-orange-50 transition-colors">
            Shop Sale
          </Link>
        </div>
      </section>
    </div>
  )
}
