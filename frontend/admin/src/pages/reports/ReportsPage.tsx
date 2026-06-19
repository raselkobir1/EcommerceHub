import { useQuery } from '@tanstack/react-query'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from 'recharts'
import { reportService } from '@/services/reportService'
import { formatBDT, formatDate } from '@/lib/utils'
import { Download } from 'lucide-react'

export default function ReportsPage() {
  const { data: summary, isLoading: summaryLoading } = useQuery({
    queryKey: ['reports', 'summary'],
    queryFn: () => reportService.getReportSummary(),
  })

  const { data: dailyRevenue, isLoading: dailyLoading } = useQuery({
    queryKey: ['reports', 'daily-revenue'],
    queryFn: () => reportService.getDailyRevenue(30),
  })

  const { data: topProducts, isLoading: topLoading } = useQuery({
    queryKey: ['reports', 'top-products'],
    queryFn: () => reportService.getTopProducts(10),
  })

  const summaryCards = [
    {
      label: 'Total Revenue',
      value: summaryLoading ? '…' : formatBDT(summary?.totalRevenue ?? 0),
      color: 'text-green-600',
    },
    {
      label: 'Total Orders',
      value: summaryLoading ? '…' : String(summary?.totalOrders ?? 0),
      color: 'text-blue-600',
    },
    {
      label: 'Avg Order Value',
      value: summaryLoading ? '…' : formatBDT(summary?.averageOrderValue ?? 0),
      color: 'text-purple-600',
    },
    {
      label: 'Total Items Sold',
      value: summaryLoading ? '…' : String(summary?.totalItemsSold ?? 0),
      color: 'text-orange-500',
    },
  ]

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-xl border p-4 flex flex-wrap items-center gap-4">
        <h1 className="text-lg font-semibold text-gray-900">Reports</h1>
        <div className="flex gap-2 ml-auto">
          <button className="flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-50 transition-colors">
            <Download size={14} />
            Excel
          </button>
          <button className="flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-50 transition-colors">
            <Download size={14} />
            PDF
          </button>
        </div>
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        {summaryCards.map((card) => (
          <div key={card.label} className="bg-white rounded-xl border p-5">
            <p className="text-sm text-gray-500">{card.label}</p>
            <p className={`text-xl font-bold mt-1 ${card.color}`}>{card.value}</p>
          </div>
        ))}
      </div>

      {/* Revenue chart */}
      <div className="bg-white rounded-xl border p-6">
        <h2 className="text-base font-semibold text-gray-900 mb-4">Daily Revenue (Last 30 Days)</h2>
        <ResponsiveContainer width="100%" height={280}>
          <BarChart data={dailyRevenue ?? []} barSize={24}>
            <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f0f0f0" />
            <XAxis
              dataKey="date"
              axisLine={false}
              tickLine={false}
              tick={{ fontSize: 11 }}
              tickFormatter={(v) => formatDate(v)}
            />
            <YAxis
              axisLine={false}
              tickLine={false}
              tick={{ fontSize: 11 }}
              tickFormatter={(v) => `৳${(v / 1000).toFixed(0)}k`}
            />
            <Tooltip
              formatter={(v: number) => formatBDT(v)}
              labelFormatter={(l) => formatDate(l)}
            />
            <Bar dataKey="revenue" fill="#2563eb" radius={[4, 4, 0, 0]} />
          </BarChart>
        </ResponsiveContainer>
      </div>

      {/* Daily revenue table */}
      <div className="bg-white rounded-xl border p-6">
        <h2 className="text-base font-semibold text-gray-900 mb-4">Daily Revenue Breakdown</h2>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b text-left text-gray-500">
                <th className="pb-3 pr-6 font-medium">Date</th>
                <th className="pb-3 pr-6 font-medium text-right">Orders</th>
                <th className="pb-3 font-medium text-right">Revenue</th>
              </tr>
            </thead>
            <tbody>
              {dailyLoading ? (
                <tr>
                  <td colSpan={3} className="py-8 text-center text-gray-400">
                    Loading…
                  </td>
                </tr>
              ) : (dailyRevenue ?? []).length === 0 ? (
                <tr>
                  <td colSpan={3} className="py-8 text-center text-gray-400">
                    No data available
                  </td>
                </tr>
              ) : (
                (dailyRevenue ?? []).map((row) => (
                  <tr key={row.date} className="border-b last:border-0 hover:bg-gray-50">
                    <td className="py-3 pr-6 text-gray-700">{formatDate(row.date)}</td>
                    <td className="py-3 pr-6 text-right tabular-nums text-gray-700">
                      {row.orders}
                    </td>
                    <td className="py-3 text-right tabular-nums font-medium text-gray-900">
                      {formatBDT(row.revenue)}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Top products table */}
      <div className="bg-white rounded-xl border p-6">
        <h2 className="text-base font-semibold text-gray-900 mb-4">Top Products</h2>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b text-left text-gray-500">
                <th className="pb-3 pr-6 font-medium">#</th>
                <th className="pb-3 pr-6 font-medium">Product</th>
                <th className="pb-3 pr-6 font-medium">SKU</th>
                <th className="pb-3 pr-6 font-medium text-right">Units Sold</th>
                <th className="pb-3 font-medium text-right">Revenue</th>
              </tr>
            </thead>
            <tbody>
              {topLoading ? (
                <tr>
                  <td colSpan={5} className="py-8 text-center text-gray-400">
                    Loading…
                  </td>
                </tr>
              ) : (topProducts ?? []).length === 0 ? (
                <tr>
                  <td colSpan={5} className="py-8 text-center text-gray-400">
                    No data available
                  </td>
                </tr>
              ) : (
                (topProducts ?? []).map((p, idx) => (
                  <tr key={p.productId} className="border-b last:border-0 hover:bg-gray-50">
                    <td className="py-3 pr-6 text-gray-400">{idx + 1}</td>
                    <td className="py-3 pr-6 font-medium text-gray-900">{p.productName}</td>
                    <td className="py-3 pr-6 font-mono text-xs text-gray-500">{p.sku}</td>
                    <td className="py-3 pr-6 text-right tabular-nums text-gray-700">
                      {p.totalSold}
                    </td>
                    <td className="py-3 text-right tabular-nums font-medium text-gray-900">
                      {formatBDT(p.totalRevenue)}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
