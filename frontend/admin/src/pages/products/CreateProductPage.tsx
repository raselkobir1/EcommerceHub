import { useState } from 'react'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { Plus, Trash2 } from 'lucide-react'
import { productService } from '@/services/productService'
import { categoryService } from '@/services/categoryService'

const variantSchema = z.object({
  sku: z.string().min(1, 'SKU required'),
  price: z.coerce.number().positive('Price must be positive'),
  stockQuantity: z.coerce.number().min(0),
  isActive: z.boolean().default(true),
})

const schema = z.object({
  name: z.string().min(2).max(300),
  slug: z.string().min(2).regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/, 'Lowercase letters and hyphens only'),
  description: z.string().optional(),
  shortDescription: z.string().max(500).optional(),
  categoryId: z.string().uuid('Select a category'),
  brandId: z.string().optional(),
  basePrice: z.coerce.number().positive(),
  compareAtPrice: z.coerce.number().positive().optional(),
  metaTitle: z.string().max(70).optional(),
  metaDescription: z.string().max(160).optional(),
  isFeatured: z.boolean().default(false),
  tags: z.string().optional(),
  variants: z.array(variantSchema).min(1, 'Add at least one variant'),
})
type FormValues = z.infer<typeof schema>

function slugify(text: string) {
  return text.toLowerCase().replace(/[^\w\s-]/g, '').replace(/[\s_]+/g, '-').trim()
}

export default function CreateProductPage() {
  const navigate = useNavigate()
  const qc = useQueryClient()
  const [serverError, setServerError] = useState('')

  const { data: categoriesResult } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoryService.getCategories(),
  })
  const categories = categoriesResult?.items ?? []

  const { register, handleSubmit, control, setValue, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { variants: [{ sku: '', price: 0, stockQuantity: 0, isActive: true }], isFeatured: false },
  })

  const { fields, append, remove } = useFieldArray({ control, name: 'variants' })

  const { mutate, isPending } = useMutation({
    mutationFn: productService.createProduct,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['products'] })
      navigate('/products')
    },
    onError: (err: { response?: { data?: { message?: string } } }) => {
      setServerError(err.response?.data?.message ?? 'Failed to create product.')
    },
  })

  const onSubmit = (data: FormValues) => {
    const payload = {
      ...data,
      fullDescription: data.description,
      tags: data.tags ? data.tags.split(',').map((t) => t.trim()).filter(Boolean) : [],
      variants: data.variants.map((v) => ({
        ...v,
        attributes: {} as Record<string, string>,
      })),
    }
    mutate(payload)
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 max-w-4xl">
      {serverError && (
        <div className="rounded-lg bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-700">{serverError}</div>
      )}

      <div className="bg-white rounded-xl border p-6 space-y-4">
        <h2 className="text-base font-semibold text-gray-900">Basic Information</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Name *</label>
            <input {...register('name')} onChange={(e) => { register('name').onChange(e); setValue('slug', slugify(e.target.value)) }}
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
            {errors.name && <p className="mt-1 text-xs text-red-600">{errors.name.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Slug *</label>
            <input {...register('slug')} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
            {errors.slug && <p className="mt-1 text-xs text-red-600">{errors.slug.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Category *</label>
            <select {...register('categoryId')} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none">
              <option value="">Select category</option>
              {categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
            {errors.categoryId && <p className="mt-1 text-xs text-red-600">{errors.categoryId.message}</p>}
          </div>
          <div className="flex items-center gap-3">
            <input type="checkbox" id="featured" {...register('isFeatured')} className="h-4 w-4 rounded text-primary-600" />
            <label htmlFor="featured" className="text-sm font-medium text-gray-700">Mark as Featured</label>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Base Price (BDT) *</label>
            <input {...register('basePrice')} type="number" min="0" step="0.01"
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
            {errors.basePrice && <p className="mt-1 text-xs text-red-600">{errors.basePrice.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Compare-at Price (BDT)</label>
            <input {...register('compareAtPrice')} type="number" min="0" step="0.01"
              className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
          </div>
          <div className="sm:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-1">Short Description</label>
            <input {...register('shortDescription')} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
          </div>
          <div className="sm:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-1">Full Description</label>
            <textarea {...register('description')} rows={4} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none resize-none" />
          </div>
          <div className="sm:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-1">Tags (comma-separated)</label>
            <input {...register('tags')} placeholder="t-shirt, casual, summer" className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
          </div>
        </div>
      </div>

      <div className="bg-white rounded-xl border p-6 space-y-4">
        <h2 className="text-base font-semibold text-gray-900">SEO</h2>
        <div className="grid grid-cols-1 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Meta Title (max 70)</label>
            <input {...register('metaTitle')} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
            {errors.metaTitle && <p className="mt-1 text-xs text-red-600">{errors.metaTitle.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Meta Description (max 160)</label>
            <textarea {...register('metaDescription')} rows={2} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none resize-none" />
            {errors.metaDescription && <p className="mt-1 text-xs text-red-600">{errors.metaDescription.message}</p>}
          </div>
        </div>
      </div>

      <div className="bg-white rounded-xl border p-6 space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-base font-semibold text-gray-900">Variants *</h2>
          <button type="button" onClick={() => append({ sku: '', price: 0, stockQuantity: 0, isActive: true })}
            className="flex items-center gap-1 rounded-lg border border-primary-600 px-3 py-1.5 text-sm text-primary-600 hover:bg-primary-50 transition-colors">
            <Plus size={14} />Add Variant
          </button>
        </div>
        {errors.variants?.root && <p className="text-xs text-red-600">{errors.variants.root.message}</p>}
        <div className="space-y-3">
          {fields.map((field, idx) => (
            <div key={field.id} className="grid grid-cols-12 gap-3 items-end">
              <div className="col-span-4">
                <label className="block text-xs font-medium text-gray-500 mb-1">SKU *</label>
                <input {...register(`variants.${idx}.sku`)} className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
                {errors.variants?.[idx]?.sku && <p className="mt-0.5 text-xs text-red-600">{errors.variants[idx].sku?.message}</p>}
              </div>
              <div className="col-span-3">
                <label className="block text-xs font-medium text-gray-500 mb-1">Price *</label>
                <input {...register(`variants.${idx}.price`)} type="number" min="0" step="0.01"
                  className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
              </div>
              <div className="col-span-3">
                <label className="block text-xs font-medium text-gray-500 mb-1">Stock</label>
                <input {...register(`variants.${idx}.stockQuantity`)} type="number" min="0"
                  className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none" />
              </div>
              <div className="col-span-2 flex justify-end">
                {fields.length > 1 && (
                  <button type="button" onClick={() => remove(idx)}
                    className="rounded-lg border border-red-200 p-2 text-red-500 hover:bg-red-50 transition-colors">
                    <Trash2 size={14} />
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="flex justify-end gap-3">
        <button type="button" onClick={() => navigate('/products')}
          className="rounded-lg border px-5 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors">
          Cancel
        </button>
        <button type="submit" disabled={isPending}
          className="rounded-lg bg-primary-600 px-5 py-2 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-60 transition-colors">
          {isPending ? 'Creating...' : 'Create Product'}
        </button>
      </div>
    </form>
  )
}
