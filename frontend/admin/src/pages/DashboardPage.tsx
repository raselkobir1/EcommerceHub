import { useQuery } from '@tanstack/react-query'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from 'recharts'
import { ShoppingCart, Package, Users, TrendingUp } from 'lucide-react'
import { orderService } from '@/services/orderService'
import { formatBDT, formatDate } from '@/lib/utils'
import Badge from '@/components/ui/Badge'

const mockRevenueData = [
  { day: 'Mon', revenue: 48000 },
  { day: 'Tue', revenue: 62000 },
  { day: 'Wed', revenue: 39000 },
  { day: 'Thu', revenue: 71000 },
  { day: 'Fri', revenue: 85000 },
  { day: 'Sat', revenue: 95000 },
  { day: 'Sun', revenue: 77000 },
]

function StatCard({ label, value, icon, color }: {
  label: string; value: string; icon: React.ReactNode; color: string
}) {
  return (
    <div className="bg-white rounded-xl border p-6 flex items-center gap-4">
      <div className={`flex h-12 w-12 items-center justify-center rounded-xl ${color}`}>
        {icon}
      </div>
      <div>
        <p className="text-sm text-gray-500">{label}</p>
        <p className="text-2xl font-bold text-gray-900">{value}</p>
      </div>
    </div>
  )
}

const STATUS_COLORS: Record<string, 'success' | 'warning' | 'info' | 'danger' | 'default'> = {
  Delivered: 'success', Confirmed: 'info', Processing: 'warning',
  Shipped: 'info', Cancelled: 'danger', PendingPayment: 'default',
}

export default function DashboardPage() {
  const { data: orders } = useQuery({
    queryKey: ['orders', 'recent'],
    queryFn: () => orderService.getOrders({ page: 1, pageSize: 5 }),
  })

  return (
    <div className="space-y-6">
      {/* Stats */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Total Revenue" value="৳ 4,82,000" icon={<TrendingUp size={22} className="text-white" />} color="bg-green-500" />
        <StatCard label="Total Orders" value="1,248" icon={<ShoppingCart size={22} className="text-white" />} color="bg-blue-500" />
        <StatCard label="Total Products" value="342" icon={<Package size={22} className="text-white" />} color="bg-purple-500" />
        <StatCard label="Customers" value="5,891" icon={<Users size={22} className="text-white" />} color="bg-orange-500" />
      </div>

      {/* Chart + Recent Orders */}
      <div className="grid grid-cols-1 gap-6 xl:grid-cols-3">
        <div className="xl:col-span-2 bg-white rounded-xl border p-6">
          <h2 className="text-base font-semibold text-gray-900 mb-4">Revenue — Last 7 Days</h2>
          <ResponsiveContainer width="100%" height={240}>
            <BarChart data={mockRevenueData} barSize={28}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f0f0f0" />
              <XAxis dataKey="day" axisLine={false} tickLine={false} tick={{ fontSize: 12 }} />
              <YAxis axisLine={false} tickLine={false} tick={{ fontSize: 12 }}
                tickFormatter={(v) => `৳${(v / 1000).toFixed(0)}k`} />
              <Tooltip formatter={(v: number) => formatBDT(v)} />
              <Bar dataKey="revenue" fill="#2563eb" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div className="bg-white rounded-xl border p-6">
          <h2 className="text-base font-semibold text-gray-900 mb-4">Low Stock Alert</h2>
          <div className="space-y-3">
            {[
              { name: 'Casual T-Shirt (XS)', stock: 3 },
              { name: 'Running Shoes (42)', stock: 5 },
              { name: 'Wireless Earbuds', stock: 2 },
              { name: 'Leather Wallet', stock: 7 },
            ].map((item) => (
              <div key={item.name} className="flex items-center justify-between text-sm">
                <span className="text-gray-700 truncate max-w-[70%]">{item.name}</span>
                <Badge variant={item.stock <= 3 ? 'danger' : 'warning'}>{item.stock} left</Badge>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Recent Orders */}
      <div className="bg-white rounded-xl border">
        <div className="flex items-center justify-between px-6 py-4 border-b">
          <h2 className="text-base font-semibold text-gray-900">Recent Orders</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-gray-50">
                <th className="px-6 py-3 text-left font-medium text-gray-500">Order #</th>
                <th className="px-6 py-3 text-left font-medium text-gray-500">Customer</th>
                <th className="px-6 py-3 text-left font-medium text-gray-500">Status</th>
                <th className="px-6 py-3 text-right font-medium text-gray-500">Total</th>
                <th className="px-6 py-3 text-left font-medium text-gray-500">Date</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {orders?.items.map((order) => (
                <tr key={order.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-3 font-mono text-primary-600">{order.orderNumber}</td>
                  <td className="px-6 py-3 text-gray-700">{order.customerName}</td>
                  <td className="px-6 py-3">
                    <Badge variant={STATUS_COLORS[order.status] ?? 'default'}>{order.status}</Badge>
                  </td>
                  <td className="px-6 py-3 text-right font-medium">{formatBDT(order.grandTotal)}</td>
                  <td className="px-6 py-3 text-gray-500">{formatDate(order.createdAt)}</td>
                </tr>
              )) ?? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-gray-400">No recent orders</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}
