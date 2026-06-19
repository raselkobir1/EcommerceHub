import Link from 'next/link'
import { MapPin, Phone, Mail } from 'lucide-react'

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-400 mt-16">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-12">
        <div className="grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-4">
          {/* Brand */}
          <div className="space-y-4">
            <h2 className="text-lg font-bold text-white">EcommerceHub</h2>
            <p className="text-sm leading-relaxed">
              Bangladesh&apos;s trusted online marketplace delivering quality products across the country.
            </p>
            <div className="space-y-2 text-sm">
              <div className="flex items-start gap-2">
                <MapPin size={14} className="mt-0.5 flex-shrink-0 text-primary-400" />
                <span>House 12, Road 4, Dhanmondi, Dhaka-1205</span>
              </div>
              <div className="flex items-center gap-2">
                <Phone size={14} className="flex-shrink-0 text-primary-400" />
                <span>+8801XXXXXXXXX</span>
              </div>
              <div className="flex items-center gap-2">
                <Mail size={14} className="flex-shrink-0 text-primary-400" />
                <span>support@ecommercehub.com.bd</span>
              </div>
            </div>
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="text-sm font-semibold uppercase tracking-wider text-white mb-4">Quick Links</h3>
            <ul className="space-y-2 text-sm">
              {[['Home', '/'], ['Products', '/products'], ['Cart', '/cart'], ['My Orders', '/account/orders']].map(([label, href]) => (
                <li key={href}><Link href={href} className="hover:text-white transition-colors">{label}</Link></li>
              ))}
            </ul>
          </div>

          {/* Help */}
          <div>
            <h3 className="text-sm font-semibold uppercase tracking-wider text-white mb-4">Help</h3>
            <ul className="space-y-2 text-sm">
              {[['FAQ', '/faq'], ['Shipping Policy', '/shipping'], ['Return Policy', '/returns'], ['Privacy Policy', '/privacy'], ['Terms of Service', '/terms']].map(([label, href]) => (
                <li key={href}><Link href={href} className="hover:text-white transition-colors">{label}</Link></li>
              ))}
            </ul>
          </div>

          {/* Payment */}
          <div>
            <h3 className="text-sm font-semibold uppercase tracking-wider text-white mb-4">We Accept</h3>
            <div className="grid grid-cols-3 gap-2">
              {['bKash', 'Nagad', 'SSLCommerz', 'ShurjoPay', 'COD'].map((method) => (
                <span key={method} className="flex items-center justify-center rounded border border-gray-700 bg-gray-800 px-2 py-1.5 text-xs font-medium text-gray-300">
                  {method}
                </span>
              ))}
            </div>
            <p className="mt-4 text-xs">Secure payments powered by Bangladesh&apos;s trusted gateways.</p>
          </div>
        </div>

        <div className="mt-10 border-t border-gray-800 pt-6 text-center text-xs">
          © {new Date().getFullYear()} EcommerceHub. All rights reserved. Made with ❤️ in Bangladesh.
        </div>
      </div>
    </footer>
  )
}
