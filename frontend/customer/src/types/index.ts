export interface ApiResponse<T> {
  isSuccess: boolean
  data: T
  message?: string
  errors?: string[]
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface Product {
  id: string
  name: string
  slug: string
  shortDescription?: string
  fullDescription?: string
  price: number
  salePrice?: number
  sku: string
  status: string
  stockQuantity: number
  categoryId: string
  categoryName?: string
  brandId?: string
  brandName?: string
  images: ProductImage[]
  variants: ProductVariant[]
  averageRating?: number
  reviewCount?: number
}

export interface ProductImage {
  id: string
  url: string
  altText?: string
  isPrimary: boolean
  displayOrder: number
}

export interface ProductVariant {
  id: string
  sku: string
  price: number
  salePrice?: number
  stockQuantity: number
  attributes: Record<string, string>
}

export interface Category {
  id: string
  name: string
  slug: string
  parentId?: string
  children?: Category[]
  imageUrl?: string
}

export interface CartItem {
  productId: string
  productName: string
  productSlug: string
  imageUrl?: string
  variantId?: string
  variantAttributes?: Record<string, string>
  price: number
  quantity: number
}

export interface Cart {
  id: string
  items: CartItem[]
  subtotal: number
  discountAmount: number
  total: number
  couponCode?: string
}

export interface Customer {
  id: string
  fullName: string
  email: string
  phone?: string
}

export interface CustomerAddress {
  id: string
  label: string
  recipientName: string
  phone: string
  addressLine1: string
  addressLine2?: string
  areaThana: string
  district: string
  division: string
  isDefault: boolean
}

export interface Order {
  id: string
  orderNumber: string
  status: string
  paymentStatus: string
  paymentMethod: string
  subtotal: number
  discountAmount: number
  shippingFee: number
  total: number
  items: OrderItem[]
  shippingAddress: ShippingAddress
  placedAt: string
  estimatedDelivery?: string
}

export interface OrderItem {
  productId: string
  productName: string
  imageUrl?: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface ShippingAddress {
  recipientName: string
  phone: string
  addressLine1: string
  addressLine2?: string
  areaThana: string
  district: string
  division: string
}

export interface Review {
  id: string
  rating: number
  title?: string
  body?: string
  isVerifiedPurchase: boolean
  createdAt: string
  customerName: string
}
