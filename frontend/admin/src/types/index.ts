// ─── Generic API wrappers ─────────────────────────────────────────────────────

export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors?: string[]
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

// ─── Auth ─────────────────────────────────────────────────────────────────────

export interface AuthTokenDto {
  accessToken: string
  refreshToken: string
  userId: string
  email: string
  fullName: string
  role: string
}

export interface AdminUserDto {
  id: string
  email: string
  fullName: string
  role: string
  isActive: boolean
  createdAt: string
}

// ─── Category & Brand ─────────────────────────────────────────────────────────

export interface CategoryDto {
  id: string
  name: string
  slug: string
  description: string | null
  imageUrl: string | null
  parentId: string | null
  isActive: boolean
  sortOrder: number
  productCount: number
  children: CategoryDto[]
}

export interface BrandDto {
  id: string
  name: string
  slug: string
  logoUrl: string | null
  isActive: boolean
  productCount: number
}

// ─── Products ─────────────────────────────────────────────────────────────────

export type ProductStatus = 'Draft' | 'Active' | 'Archived'

export interface ProductVariantDto {
  id: string
  sku: string
  price: number
  compareAtPrice: number | null
  stockQuantity: number
  reservedQuantity: number
  attributes: Record<string, string>
  isActive: boolean
}

export interface ProductListDto {
  id: string
  name: string
  slug: string
  sku: string
  categoryId: string
  categoryName: string
  brandId: string | null
  brandName: string | null
  basePrice: number
  compareAtPrice: number | null
  thumbnailUrl: string | null
  status: ProductStatus
  isFeatured: boolean
  totalStock: number
  createdAt: string
}

export interface ProductDetailDto extends ProductListDto {
  shortDescription: string | null
  fullDescription: string | null
  metaTitle: string | null
  metaDescription: string | null
  tags: string[]
  variants: ProductVariantDto[]
  images: ProductImageDto[]
  updatedAt: string
}

export interface ProductImageDto {
  id: string
  url: string
  altText: string | null
  sortOrder: number
  isPrimary: boolean
}

// ─── Orders ───────────────────────────────────────────────────────────────────

export type OrderStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Processing'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled'
  | 'Returned'
  | 'Refunded'

export type PaymentStatus = 'Pending' | 'Paid' | 'Failed' | 'Refunded' | 'PartiallyRefunded'

export interface OrderItemDto {
  id: string
  productId: string
  productName: string
  variantId: string | null
  sku: string
  quantity: number
  unitPrice: number
  discount: number
  totalPrice: number
  thumbnailUrl: string | null
}

export interface OrderAddressDto {
  fullName: string
  phone: string
  addressLine1: string
  addressLine2: string | null
  city: string
  district: string
  postalCode: string | null
}

export interface OrderSummaryDto {
  id: string
  orderNumber: string
  customerId: string
  customerName: string
  customerEmail: string
  customerPhone: string
  status: OrderStatus
  paymentStatus: PaymentStatus
  paymentMethod: string
  subtotal: number
  discountAmount: number
  shippingFee: number
  taxAmount: number
  grandTotal: number
  createdAt: string
  updatedAt: string
}

export interface OrderDto extends OrderSummaryDto {
  items: OrderItemDto[]
  shippingAddress: OrderAddressDto
  billingAddress: OrderAddressDto | null
  couponCode: string | null
  notes: string | null
  trackingNumber: string | null
  statusHistory: OrderStatusHistoryDto[]
}

export interface OrderStatusHistoryDto {
  id: string
  status: OrderStatus
  note: string | null
  createdAt: string
  createdBy: string
}

// ─── Inventory ────────────────────────────────────────────────────────────────

export type AdjustmentReason =
  | 'Purchase'
  | 'Sale'
  | 'Return'
  | 'Damage'
  | 'Correction'
  | 'Transfer'
  | 'Initial'

export interface InventoryItemDto {
  productId: string
  variantId: string
  productName: string
  sku: string
  stockQuantity: number
  reservedQuantity: number
  availableQuantity: number
  reorderPoint: number
  isLowStock: boolean
  lastUpdatedAt: string
}

export interface StockAdjustmentDto {
  id: string
  productId: string
  variantId: string
  productName: string
  sku: string
  quantityChange: number
  oldStock: number
  newStock: number
  reason: AdjustmentReason
  notes: string | null
  adjustedByUserId: string
  createdAt: string
}

// ─── Coupons ──────────────────────────────────────────────────────────────────

export type DiscountType = 'Percentage' | 'FixedAmount' | 'FreeShipping'

export interface CouponDto {
  id: string
  code: string
  description: string | null
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount: number | null
  maximumDiscountAmount: number | null
  usageLimit: number | null
  usageCount: number
  isActive: boolean
  startsAt: string
  expiresAt: string | null
  createdAt: string
}

// ─── Promotions / Banners ─────────────────────────────────────────────────────

export type BannerPosition = 'HeroSlider' | 'HomeMidBanner' | 'SidebarTop' | 'Footer'

export interface BannerDto {
  id: string
  title: string
  imageUrl: string
  linkUrl: string | null
  position: BannerPosition
  sortOrder: number
  isActive: boolean
  startsAt: string | null
  expiresAt: string | null
}

// ─── Suppliers & Purchase Orders ──────────────────────────────────────────────

export interface SupplierDto {
  id: string
  name: string
  contactPerson: string | null
  email: string | null
  phone: string | null
  address: string | null
  taxId: string | null
  paymentTerms: string | null
  currentBalance: number
  isActive: boolean
  createdAt: string
}

export type PurchaseOrderStatus = 'Draft' | 'Sent' | 'PartiallyReceived' | 'Received' | 'Cancelled'

export interface PurchaseOrderItemDto {
  id: string
  productVariantId: string
  sku: string
  productName: string
  orderedQty: number
  receivedQty: number
  unitCost: number
  totalCost: number
}

export interface PurchaseOrderDto {
  id: string
  poNumber: string
  supplierId: string
  supplierName: string
  status: PurchaseOrderStatus
  orderDate: string
  expectedDeliveryDate: string | null
  subtotal: number
  taxAmount: number
  shippingCost: number
  grandTotal: number
  notes: string | null
  items: PurchaseOrderItemDto[]
  createdAt: string
}

// ─── Reviews ──────────────────────────────────────────────────────────────────

export type ReviewStatus = 'Pending' | 'Approved' | 'Rejected'

export interface ReviewDto {
  id: string
  productId: string
  productName: string
  customerId: string
  customerName: string
  rating: number
  title: string | null
  body: string | null
  isVerifiedPurchase: boolean
  status: ReviewStatus
  helpfulVotes: number
  createdAt: string
}

// ─── Dashboard ────────────────────────────────────────────────────────────────

export interface DashboardStatsDto {
  totalRevenueBDT: number
  totalOrders: number
  totalProducts: number
  totalCustomers: number
  revenueGrowthPct: number
  ordersGrowthPct: number
}

export interface DailyOrderCountDto {
  date: string
  orderCount: number
  revenue: number
}

// ─── Reports ──────────────────────────────────────────────────────────────────

export interface ReportSummaryDto {
  totalRevenue: number
  totalOrders: number
  averageOrderValue: number
  totalItemsSold: number
  newCustomers: number
  revenueGrowthPercent: number
}

export interface DailyRevenueDto {
  date: string
  revenue: number
  orders: number
}

export interface TopProductDto {
  productId: string
  productName: string
  sku: string
  totalSold: number
  totalRevenue: number
}
