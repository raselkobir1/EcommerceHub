'use client'
import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Pencil, Trash2 } from 'lucide-react'
import { categoryService, type CreateCategoryRequest } from '@/services/categoryService'
import type { CategoryDto } from '@/types'
import Badge from '@/components/ui/Badge'
import Modal from '@/components/ui/Modal'

// Flatten the category tree and add parentName for table display
interface FlatCategory extends CategoryDto {
  parentName: string | null
}

function flattenTree(nodes: CategoryDto[], parentName: string | null = null): FlatCategory[] {
  return nodes.flatMap((node) => [
    { ...node, parentName },
    ...flattenTree(node.children ?? [], node.name),
  ])
}

const EMPTY_FORM: CreateCategoryRequest = {
  name: '',
  slug: '',
  parentId: null,
  description: '',
  imageUrl: '',
  sortOrder: 0,
}

function toSlug(name: string) {
  return name
    .toLowerCase()
    .trim()
    .replace(/[^\w\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
}

export default function CategoriesPage() {
  const qc = useQueryClient()
  const [showModal, setShowModal] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [form, setForm] = useState<CreateCategoryRequest>(EMPTY_FORM)
  const [slugManual, setSlugManual] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const { data: tree, isLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoryService.getCategories(),
  })

  const categories = flattenTree(tree ?? [])

  const createMutation = useMutation({
    mutationFn: categoryService.createCategory,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['categories'] })
      closeModal()
    },
    onError: (err: unknown) => {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
      setError(msg ?? 'Failed to create category.')
    },
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<CreateCategoryRequest> }) =>
      categoryService.updateCategory(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['categories'] })
      closeModal()
    },
    onError: (err: unknown) => {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
      setError(msg ?? 'Failed to update category.')
    },
  })

  const deleteMutation = useMutation({
    mutationFn: categoryService.deleteCategory,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['categories'] }),
    onError: (err: unknown) => {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
      alert(msg ?? 'Failed to delete category.')
    },
  })

  function openAdd() {
    setEditId(null)
    setForm(EMPTY_FORM)
    setSlugManual(false)
    setError(null)
    setShowModal(true)
  }

  function openEdit(cat: FlatCategory) {
    setEditId(cat.id)
    setForm({
      name: cat.name,
      slug: cat.slug,
      parentId: cat.parentId,
      description: cat.description ?? '',
      imageUrl: cat.imageUrl ?? '',
      sortOrder: cat.sortOrder,
    })
    setSlugManual(true)
    setError(null)
    setShowModal(true)
  }

  function closeModal() {
    setShowModal(false)
    setEditId(null)
    setForm(EMPTY_FORM)
    setSlugManual(false)
    setError(null)
  }

  function handleNameChange(name: string) {
    setForm((f) => ({
      ...f,
      name,
      slug: slugManual ? f.slug : toSlug(name),
    }))
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)

    if (!form.name.trim()) { setError('Name is required.'); return }
    if (!form.slug.trim()) { setError('Slug is required.'); return }

    const payload: CreateCategoryRequest = {
      name: form.name.trim(),
      slug: form.slug.trim(),
      parentId: form.parentId || null,
      description: form.description?.trim() || undefined,
      imageUrl: form.imageUrl?.trim() || undefined,
      sortOrder: form.sortOrder ?? 0,
    }

    if (editId) {
      updateMutation.mutate({ id: editId, data: payload })
    } else {
      createMutation.mutate(payload)
    }
  }

  const isPending = createMutation.isPending || updateMutation.isPending

  // All categories available for parent selection (excluding current edit target and its descendants)
  const parentOptions = categories.filter((c) => c.id !== editId)

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <p className="text-sm text-gray-500">{categories.length} categories</p>
        <button
          onClick={openAdd}
          className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 transition-colors"
        >
          + Add Category
        </button>
      </div>

      <div className="bg-white rounded-xl border overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-gray-50">
              <th className="px-4 py-3 text-left font-medium text-gray-500">Name</th>
              <th className="px-4 py-3 text-left font-medium text-gray-500">Slug</th>
              <th className="px-4 py-3 text-left font-medium text-gray-500">Parent</th>
              <th className="px-4 py-3 text-center font-medium text-gray-500">Products</th>
              <th className="px-4 py-3 text-center font-medium text-gray-500">Status</th>
              <th className="px-4 py-3 text-center font-medium text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {isLoading && Array.from({ length: 5 }).map((_, i) => (
              <tr key={i}>{Array.from({ length: 6 }).map((_, j) => (
                <td key={j} className="px-4 py-3">
                  <div className="h-4 rounded bg-gray-200 animate-pulse" />
                </td>
              ))}</tr>
            ))}
            {!isLoading && categories.map((cat) => (
              <tr key={cat.id} className="hover:bg-gray-50 transition-colors">
                <td className="px-4 py-3 font-medium text-gray-900">
                  {cat.parentId && <span className="mr-1 text-gray-300">↳</span>}
                  {cat.name}
                </td>
                <td className="px-4 py-3 font-mono text-xs text-gray-500">{cat.slug}</td>
                <td className="px-4 py-3 text-gray-500">{cat.parentName ?? '—'}</td>
                <td className="px-4 py-3 text-center text-gray-600">{cat.productCount}</td>
                <td className="px-4 py-3 text-center">
                  <Badge variant={cat.isActive ? 'success' : 'default'}>
                    {cat.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                </td>
                <td className="px-4 py-3 text-center">
                  <div className="flex items-center justify-center gap-2">
                    <button
                      onClick={() => openEdit(cat)}
                      className="rounded p-1 text-gray-400 hover:bg-blue-50 hover:text-blue-600 transition-colors"
                      title="Edit"
                    >
                      <Pencil size={14} />
                    </button>
                    <button
                      onClick={() => {
                        if (confirm(`Delete "${cat.name}"?`)) deleteMutation.mutate(cat.id)
                      }}
                      className="rounded p-1 text-gray-400 hover:bg-red-50 hover:text-red-600 transition-colors"
                      title="Delete"
                    >
                      <Trash2 size={14} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {!isLoading && categories.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-10 text-center text-gray-400">
                  No categories found
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal
        isOpen={showModal}
        onClose={closeModal}
        title={editId ? 'Edit Category' : 'Add Category'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          {error && (
            <div className="rounded-lg bg-red-50 border border-red-200 px-3 py-2 text-sm text-red-700">
              {error}
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Name <span className="text-red-500">*</span>
            </label>
            <input
              value={form.name}
              onChange={(e) => handleNameChange(e.target.value)}
              placeholder="e.g. Electronics"
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Slug <span className="text-red-500">*</span>
            </label>
            <input
              value={form.slug}
              onChange={(e) => {
                setSlugManual(true)
                setForm((f) => ({ ...f, slug: e.target.value }))
              }}
              placeholder="e.g. electronics"
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm font-mono focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Parent Category</label>
            <select
              value={form.parentId ?? ''}
              onChange={(e) => setForm((f) => ({ ...f, parentId: e.target.value || null }))}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            >
              <option value="">— None (root category) —</option>
              {parentOptions.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.parentName ? `${c.parentName} › ${c.name}` : c.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <textarea
              value={form.description ?? ''}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
              rows={2}
              placeholder="Optional description"
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm resize-none focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Image URL</label>
            <input
              value={form.imageUrl ?? ''}
              onChange={(e) => setForm((f) => ({ ...f, imageUrl: e.target.value }))}
              placeholder="https://..."
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Sort Order</label>
            <input
              type="number"
              value={form.sortOrder ?? 0}
              onChange={(e) => setForm((f) => ({ ...f, sortOrder: Number(e.target.value) }))}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
            />
          </div>

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={closeModal}
              className="rounded-lg border border-gray-300 px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isPending}
              className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-60 transition-colors"
            >
              {isPending ? 'Saving…' : editId ? 'Save Changes' : 'Add Category'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
