import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { inventoryService, AdjustStockRequest } from '@/services/inventoryService'
import { AdjustmentReason } from '@/types'
import Badge from '@/components/ui/Badge'
import Modal from '@/components/ui/Modal'
import { AlertTriangle } from 'lucide-react'

const REASONS: AdjustmentReason[] = ['Purchase', 'Return', 'Damage', 'Correction', 'Transfer', 'Initial']

const emptyForm = (): AdjustStockRequest => ({
  variantId: '',
  reason: 'Correction',
  quantityChange: 0,
  notes: '',
})

export default function InventoryPage() {
  const [showLowStock, setShowLowStock] = useState(false)
  const [adjustModal, setAdjustModal] = useState(false)
  const [form, setForm] = useState<AdjustStockRequest>(emptyForm())
  const qc = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['inventory'],
    queryFn: () => inventoryService.getInventory({ lowStock: showLowStock }),
  })

  const { mutate, isPending } = useMutation({
    mutationFn: (req: AdjustStockRequest) => inventoryService.adjustStock(req),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['inventory'] })
      setAdjustModal(false)
      setForm(emptyForm())
    },
  })

  const items = (data?.items ?? []).filter((a) => !showLowStock || a.availableQuantity <= 10)

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <label className="flex items-center gap-2 text-sm text-gray-600 cursor-pointer">
          <input type="checkbox" checked={showLowStock}
            onChange={(e) => setShowLowStock(e.target.checked)} className="rounded" />
          Show low stock only (≤10)
        </label>
        <button onClick={() => setAdjustModal(true)}
          className="flex items-center gap-2 rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 transition-colors">
          + Stock Adjustment
        </button>
      </div>

      <div className="bg-white rounded-xl border overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-gray-50">
                <th className="px-4 py-3 text-left font-medium text-gray-500">Product</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">SKU</th>
                <th className="px-4 py-3 text-center font-medium text-gray-500">Stock</th>
                <th className="px-4 py-3 text-center font-medium text-gray-500">Reserved</th>
                <th className="px-4 py-3 text-center font-medium text-gray-500">Available</th>
                <th className="px-4 py-3 text-left font-medium text-gray-500">Reorder At</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {isLoading && Array.from({ length: 6 }).map((_, i) => (
                <tr key={i}>{Array.from({ length: 6 }).map((_, j) => (
                  <td key={j} className="px-4 py-3"><div className="h-4 bg-gray-200 rounded animate-pulse" /></td>
                ))}</tr>
              ))}
              {!isLoading && items.map((item) => (
                <tr key={item.variantId} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-900">{item.productName}</td>
                  <td className="px-4 py-3 font-mono text-xs text-gray-500">{item.sku}</td>
                  <td className="px-4 py-3 text-center text-gray-700">{item.stockQuantity}</td>
                  <td className="px-4 py-3 text-center text-gray-500">{item.reservedQuantity}</td>
                  <td className="px-4 py-3 text-center">
                    <Badge variant={item.availableQuantity <= 5 ? 'danger' : item.availableQuantity <= 10 ? 'warning' : 'success'}>
                      {item.availableQuantity}
                    </Badge>
                  </td>
                  <td className="px-4 py-3">
                    <span className="inline-flex items-center gap-1 text-gray-600">
                      {item.reorderPoint !== null && item.availableQuantity <= item.reorderPoint && (
                        <AlertTriangle size={12} className="text-red-400" />
                      )}
                      {item.reorderPoint ?? '—'}
                    </span>
                  </td>
                </tr>
              ))}
              {!isLoading && items.length === 0 && (
                <tr><td colSpan={6} className="px-4 py-10 text-center text-gray-400">No inventory items</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      <Modal isOpen={adjustModal} onClose={() => setAdjustModal(false)} title="Stock Adjustment">
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Variant ID</label>
            <input value={form.variantId}
              onChange={(e) => setForm({ ...form, variantId: e.target.value })}
              placeholder="Paste variant UUID"
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Quantity Change (+/-)</label>
            <input type="number" value={form.quantityChange}
              onChange={(e) => setForm({ ...form, quantityChange: Number(e.target.value) })}
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Reason</label>
            <select value={form.reason}
              onChange={(e) => setForm({ ...form, reason: e.target.value as AdjustmentReason })}
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none">
              {REASONS.map((r) => <option key={r}>{r}</option>)}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Notes</label>
            <textarea value={form.notes ?? ''}
              onChange={(e) => setForm({ ...form, notes: e.target.value })} rows={2}
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none resize-none" />
          </div>
          <div className="flex justify-end gap-2">
            <button onClick={() => setAdjustModal(false)}
              className="rounded-lg border px-4 py-2 text-sm text-gray-700 hover:bg-gray-50">Cancel</button>
            <button disabled={isPending || !form.variantId} onClick={() => mutate(form)}
              className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-60">
              {isPending ? 'Saving...' : 'Save Adjustment'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  )
}
