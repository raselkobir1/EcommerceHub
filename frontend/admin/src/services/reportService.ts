import { api } from '@/lib/api'
import type { ApiResponse, DailyOrderCountDto, DailyRevenueDto, DashboardStatsDto, ReportSummaryDto } from '@/types'

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

  getReportSummary: (fromDate: string, toDate: string) =>
    api
      .get<ApiResponse<ReportSummaryDto>>('/reports/summary', {
        params: { fromDate, toDate },
      })
      .then((r) => r.data.data),

  getDailyRevenue: (fromDate: string, toDate: string) =>
    api
      .get<ApiResponse<DailyRevenueDto[]>>('/reports/daily-revenue', {
        params: { fromDate, toDate },
      })
      .then((r) => r.data.data),
}
