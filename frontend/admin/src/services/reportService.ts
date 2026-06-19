import { api } from '@/lib/api'
import type {
  ApiResponse,
  DailyOrderCountDto,
  DailyRevenueDto,
  DashboardStatsDto,
  ReportSummaryDto,
  TopProductDto,
} from '@/types'

export const reportService = {
  getDashboardStats: () =>
    api
      .get<ApiResponse<DashboardStatsDto>>('/reports/dashboard-stats')
      .then((r) => r.data.data),

  getLast7DaysOrders: () =>
    api
      .get<ApiResponse<DailyOrderCountDto[]>>('/reports/last-7-days')
      .then((r) => r.data.data),

  getRecentOrders: () =>
    api
      .get<ApiResponse<import('@/types').OrderSummaryDto[]>>('/orders', {
        params: { page: 1, pageSize: 5, sortBy: 'createdAt', sortDirection: 'desc' },
      })
      .then((r) => r.data.data),

  // GET /api/reports/summary — no date params; backend returns period-fixed summary
  getReportSummary: () =>
    api
      .get<ApiResponse<ReportSummaryDto>>('/reports/summary')
      .then((r) => r.data.data),

  // GET /api/reports/daily-revenue?days=30
  getDailyRevenue: (days = 30) =>
    api
      .get<ApiResponse<DailyRevenueDto[]>>('/reports/daily-revenue', {
        params: { days },
      })
      .then((r) => r.data.data),

  // GET /api/reports/top-products?limit=10
  getTopProducts: (limit = 10) =>
    api
      .get<ApiResponse<TopProductDto[]>>('/reports/top-products', {
        params: { limit },
      })
      .then((r) => r.data.data),
}
