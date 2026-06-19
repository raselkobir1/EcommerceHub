import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from 'recharts'
import { reportService } from '@/services/reportService'
import { formatBDT, formatDate } from '@/lib/utils'
import { Download } from 'lucide-react'

export default function ReportsPage() {
  const today = new Date()
  const thirtyDaysAgo = new Date(today)
  thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30)

  const [from, setFrom] = useState(thirtyDaysAgo.toISOString().split('T')[0])
  const [to, setTo] = useState(today.toISOString().split('T')[0])

  const { data: summary, isLoading } = useQuery({
    queryKey: ['reports', 'summary', from, to],
    queryFn: () => reportService.getReportSummary(from, to),
  })

  const { data: dailyRevenue } = useQuery({
    queryKey: ['reports', 'daily-revenue', from, to],
    queryFn: () => reportService.getDailyRevenue(from, to),
  })

  return (
    <div className="space-y-6">
      {/* Date filter */}
      <div className="bg-white rounded-xl border p-4 flex flex-wrap items-center gap-4">
        <div className="flex items-center gap-2">
          <label className="text-sm font-medium text-gray-700">From</label>
          <input type="date" value={from} onChange={(e) => setFrom(e.target.value)}
            className="rounded-lg border px-3 py-1.5 text-sm focus:outline-none" />
        </div>
        <div className="flex items-center gap-2">
          <label className="text-sm font-medium text-gray-700">To</label>
          <input type="date" value={to} onChange={(e) => setTo(e.target.value)}
            className="rounded-lg border px-3 py-1.5 text-sm focus:outline-none" />
        </div>
        <div className="flex gap-2 ml-auto">
          <button className="flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-50 transition-colors">
            <Download size={14} />Excel
          </button>
          <button className="flex items-center gap-1.5 rounded-lg border px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-50 transition-colors">
            <Download size={14} />PDF
          </button>
        </div>
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        {[
          { label: 'Total Revenue', value: isLoading ? '…' : formatBDT(summary?.totalRevenue ?? 0), color: 'text-green-600' },
          { label: 'Orders', value: isLoading ? '…' : String(summary?.totalOrders ?? 0), color: 'text-blue-600' },
          { label: 'Avg Order Value', value: isLoading ? '…' : formatBDT(summary?.averageOrderValue ?? 0), color: 'text-purple-600' },
          { label: 'Items Sold', value: isLoading ? '…' : String(summary?.totalItemsSold ?? 0), color: 'text-orange-500' },
        ].map((card) => (
          <div key={card.label} className="bg-white rounded-xl border p-5">
            <p className="text-sm text-gray-500">{card.label}</p>
            <p className={`text-xl font-bold mt-1 ${card.color}`}>{card.value}</p>
          </div>
        ))}
      </div>

      {/* Revenue chart */}
      <div className="bg-white rounded-xl border p-6">
        <h2 className="text-base font-semibold text-gray-900 mb-4">Daily Revenue</h2>
        <ResponsiveContainer width="100%" height={280}>
          <BarChart data={dailyRevenue ?? []} barSize={24}>
            <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f0f0f0" />
            <XAxis dataKey="date" axisLine={false} tickLine={false} tick={{ fontSize: 11 }}
              tickFormatter={(v) => formatDate(v)} />
            <YAxis axisLine={false} tickLine={false} tick={{ fontSize: 11 }}
              tickFormatter={(v) => `৳${(v / 1000).toFixed(0)}k`} />
            <Tooltip formatter={(v: number) => formatBDT(v)} labelFormatter={(l) => formatDate(l)} />
            <Bar dataKey="revenue" fill="#2563eb" radius={[4, 4, 0, 0]} />
          </BarChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}
