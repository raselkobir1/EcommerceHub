'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { ArrowLeft, Plus, Trash2, MapPin, Star, Pencil, X, CheckCircle } from 'lucide-react'
import { useAuthStore } from '@/stores/authStore'
import { addressService } from '@/services/addressService'
import type { CustomerAddress } from '@/types'
import { cn } from '@/lib/utils'

// ─── constants ────────────────────────────────────────────────────────────────

const BD_PHONE = /^(\+8801|01)[3-9]\d{8}$/

const BD_DIVISIONS = [
  'Dhaka',
  'Chittagong',
  'Rajshahi',
  'Khulna',
  'Barisal',
  'Sylhet',
  'Rangpur',
  'Mymensingh',
]

// ─── zod schema ───────────────────────────────────────────────────────────────

const schema = z.object({
  label: z.string().min(1, 'Label is required'),
  recipientName: z.string().min(2, 'Recipient name is required'),
  phone: z.string().regex(BD_PHONE, 'Enter a valid BD phone number (e.g. 01712345678)'),
  addressLine1: z.string().min(5, 'Street address is required'),
  addressLine2: z.string().optional(),
  areaThana: z.string().min(2, 'Area / Thana is required'),
  district: z.string().min(2, 'District is required'),
  division: z.string().min(2, 'Division is required'),
  isDefault: z.boolean().default(false),
})

type FormData = z.infer<typeof schema>

// ─── toast notification ───────────────────────────────────────────────────────

interface ToastProps {
  message: string
  type: 'success' | 'error'
}

function Toast({ message, type }: ToastProps) {
  return (
    <div
      className={cn(
        'fixed bottom-6 right-6 z-[60] flex items-center gap-2 rounded-xl px-4 py-3 text-sm font-medium shadow-lg transition-all',
        type === 'success'
          ? 'bg-green-600 text-white'
          : 'bg-red-600 text-white'
      )}
    >
      <CheckCircle size={16} />
      {message}
    </div>
  )
}

// ─── small reusable field wrapper ─────────────────────────────────────────────

function Field({
  label,
  error,
  children,
  required,
}: {
  label: string
  error?: string
  children: React.ReactNode
  required?: boolean
}) {
  return (
    <div className="flex flex-col gap-1">
      <label className="text-sm font-medium text-gray-700">
        {label}
        {required && <span className="ml-0.5 text-red-500">*</span>}
      </label>
      {children}
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  )
}

const INPUT_BASE =
  'rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500 disabled:bg-gray-50'

// ─── add / edit modal ─────────────────────────────────────────────────────────

interface ModalProps {
  open: boolean
  onClose: () => void
  editing: CustomerAddress | null
  onSaved: (message: string) => void
}

function AddressModal({ open, onClose, editing, onSaved }: ModalProps) {
  const qc = useQueryClient()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      label: '',
      recipientName: '',
      phone: '',
      addressLine1: '',
      addressLine2: '',
      areaThana: '',
      district: '',
      division: '',
      isDefault: false,
    },
  })

  // Populate form when editing an existing address
  useEffect(() => {
    if (editing) {
      reset({
        label: editing.label,
        recipientName: editing.recipientName,
        phone: editing.phone,
        addressLine1: editing.addressLine1,
        addressLine2: editing.addressLine2 ?? '',
        areaThana: editing.areaThana,
        district: editing.district,
        division: editing.division,
        isDefault: editing.isDefault,
      })
    } else {
      reset({
        label: '',
        recipientName: '',
        phone: '',
        addressLine1: '',
        addressLine2: '',
        areaThana: '',
        district: '',
        division: '',
        isDefault: false,
      })
    }
  }, [editing, reset, open])

  const addMutation = useMutation({
    mutationFn: addressService.addAddress,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['addresses'] })
      onSaved('Address saved successfully.')
    },
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: FormData }) =>
      addressService.updateAddress(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['addresses'] })
      onSaved('Address updated successfully.')
    },
  })

  const onSubmit = async (values: FormData) => {
    if (editing) {
      await updateMutation.mutateAsync({ id: editing.id, data: values })
    } else {
      await addMutation.mutateAsync(values)
    }
  }

  const serverError = addMutation.error?.message ?? updateMutation.error?.message
  const isBusy = isSubmitting || addMutation.isPending || updateMutation.isPending

  if (!open) return null

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
      onClick={(e) => e.target === e.currentTarget && onClose()}
    >
      <div className="w-full max-w-lg rounded-2xl bg-white shadow-xl">
        {/* header */}
        <div className="flex items-center justify-between border-b px-6 py-4">
          <h2 className="text-lg font-bold text-gray-900">
            {editing ? 'Edit Address' : 'Add New Address'}
          </h2>
          <button
            onClick={onClose}
            className="rounded-lg p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600 transition-colors"
          >
            <X size={20} />
          </button>
        </div>

        {/* form body */}
        <form onSubmit={handleSubmit(onSubmit)} className="px-6 py-5 space-y-4">
          {serverError && (
            <p className="rounded-lg bg-red-50 px-4 py-2 text-sm text-red-700">{serverError}</p>
          )}

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field label="Label" error={errors.label?.message} required>
              <input
                {...register('label')}
                placeholder="e.g. Home, Office"
                className={INPUT_BASE}
              />
            </Field>

            <Field label="Recipient Name" error={errors.recipientName?.message} required>
              <input
                {...register('recipientName')}
                placeholder="Full name"
                className={INPUT_BASE}
              />
            </Field>
          </div>

          <Field label="Phone" error={errors.phone?.message} required>
            <input
              {...register('phone')}
              placeholder="01712345678"
              className={INPUT_BASE}
            />
          </Field>

          <Field label="Street Address" error={errors.addressLine1?.message} required>
            <input
              {...register('addressLine1')}
              placeholder="House no, road, block..."
              className={INPUT_BASE}
            />
          </Field>

          <Field label="Apartment / Floor" error={errors.addressLine2?.message}>
            <input
              {...register('addressLine2')}
              placeholder="Apartment, suite, floor (optional)"
              className={INPUT_BASE}
            />
          </Field>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Field label="Area / Thana" error={errors.areaThana?.message} required>
              <input
                {...register('areaThana')}
                placeholder="Thana / Upazila"
                className={INPUT_BASE}
              />
            </Field>

            <Field label="District" error={errors.district?.message} required>
              <input
                {...register('district')}
                placeholder="District"
                className={INPUT_BASE}
              />
            </Field>

            <Field label="Division" error={errors.division?.message} required>
              <select {...register('division')} className={INPUT_BASE}>
                <option value="">Select...</option>
                {BD_DIVISIONS.map((d) => (
                  <option key={d} value={d}>
                    {d}
                  </option>
                ))}
              </select>
            </Field>
          </div>

          <label className="flex cursor-pointer items-center gap-2 select-none">
            <input
              type="checkbox"
              {...register('isDefault')}
              className="h-4 w-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
            />
            <span className="text-sm text-gray-700">Set as default address</span>
          </label>

          {/* footer buttons */}
          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isBusy}
              className="rounded-lg bg-primary-600 px-5 py-2 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-60 transition-colors"
            >
              {isBusy ? 'Saving...' : editing ? 'Save Changes' : 'Save Address'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

// ─── confirm delete dialog ────────────────────────────────────────────────────

function ConfirmDialog({
  open,
  onCancel,
  onConfirm,
  loading,
}: {
  open: boolean
  onCancel: () => void
  onConfirm: () => void
  loading: boolean
}) {
  if (!open) return null
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-sm rounded-2xl bg-white p-6 shadow-xl">
        <h3 className="text-base font-bold text-gray-900">Delete address?</h3>
        <p className="mt-1 text-sm text-gray-500">This action cannot be undone.</p>
        <div className="mt-5 flex justify-end gap-3">
          <button
            onClick={onCancel}
            disabled={loading}
            className="rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            disabled={loading}
            className="rounded-lg bg-red-600 px-4 py-2 text-sm font-semibold text-white hover:bg-red-700 disabled:opacity-60 transition-colors"
          >
            {loading ? 'Deleting...' : 'Delete'}
          </button>
        </div>
      </div>
    </div>
  )
}

// ─── address card ─────────────────────────────────────────────────────────────

interface AddressCardProps {
  address: CustomerAddress
  onEdit: (a: CustomerAddress) => void
  onDelete: (id: string) => void
  onSetDefault: (id: string) => void
  isSettingDefault: boolean
  isDeleting: boolean
}

function AddressCard({
  address,
  onEdit,
  onDelete,
  onSetDefault,
  isSettingDefault,
  isDeleting,
}: AddressCardProps) {
  return (
    <div
      className={cn(
        'relative rounded-xl border bg-white p-5 shadow-sm transition-shadow hover:shadow-md',
        address.isDefault
          ? 'border-primary-400 ring-1 ring-primary-300'
          : 'border-gray-200'
      )}
    >
      {/* default badge */}
      {address.isDefault && (
        <span className="absolute right-4 top-4 inline-flex items-center gap-1 rounded-full bg-green-100 px-2.5 py-0.5 text-xs font-semibold text-green-700">
          <Star size={10} className="fill-green-500 text-green-500" />
          Default
        </span>
      )}

      {/* label badge + recipient info */}
      <div className="mb-3 flex items-start gap-2">
        <div className="mt-0.5 rounded-lg bg-gray-100 p-2">
          <MapPin size={16} className="text-gray-500" />
        </div>
        <div>
          <span className="inline-block rounded-md bg-primary-50 px-2 py-0.5 text-xs font-semibold text-primary-700 mb-1">
            {address.label}
          </span>
          <p className="text-sm font-medium text-gray-800">{address.recipientName}</p>
          <p className="text-sm text-gray-500">{address.phone}</p>
        </div>
      </div>

      {/* address lines */}
      <p className="text-sm text-gray-700">{address.addressLine1}</p>
      {address.addressLine2 && (
        <p className="text-sm text-gray-500">{address.addressLine2}</p>
      )}
      <p className="mt-0.5 text-sm text-gray-600">
        {address.areaThana}, {address.district}, {address.division}
      </p>

      {/* action buttons */}
      <div className="mt-4 flex flex-wrap items-center gap-2 border-t border-gray-100 pt-3">
        <button
          onClick={() => onEdit(address)}
          className="flex items-center gap-1.5 rounded-lg border border-gray-300 px-3 py-1.5 text-xs font-medium text-gray-700 hover:bg-gray-50 transition-colors"
        >
          <Pencil size={13} />
          Edit
        </button>

        {!address.isDefault && (
          <button
            onClick={() => onSetDefault(address.id)}
            disabled={isSettingDefault}
            className="flex items-center gap-1.5 rounded-lg border border-primary-300 px-3 py-1.5 text-xs font-medium text-primary-700 hover:bg-primary-50 disabled:opacity-50 transition-colors"
          >
            <Star size={13} />
            {isSettingDefault ? 'Updating...' : 'Set as Default'}
          </button>
        )}

        <button
          onClick={() => onDelete(address.id)}
          disabled={isDeleting}
          className="ml-auto flex items-center gap-1.5 rounded-lg border border-red-200 px-3 py-1.5 text-xs font-medium text-red-600 hover:bg-red-50 disabled:opacity-50 transition-colors"
        >
          <Trash2 size={13} />
          {isDeleting ? 'Deleting...' : 'Delete'}
        </button>
      </div>
    </div>
  )
}

// ─── loading skeleton ─────────────────────────────────────────────────────────

function SkeletonCard() {
  return (
    <div className="animate-pulse rounded-xl border border-gray-200 bg-white p-5">
      <div className="flex gap-3 mb-3">
        <div className="h-8 w-8 rounded-lg bg-gray-200" />
        <div className="flex-1 space-y-2">
          <div className="h-4 w-16 rounded bg-gray-200" />
          <div className="h-3 w-28 rounded bg-gray-200" />
          <div className="h-3 w-24 rounded bg-gray-200" />
        </div>
      </div>
      <div className="space-y-2 mt-1">
        <div className="h-3 w-full rounded bg-gray-200" />
        <div className="h-3 w-3/4 rounded bg-gray-200" />
      </div>
      <div className="mt-4 flex gap-2 border-t border-gray-100 pt-3">
        <div className="h-7 w-16 rounded-lg bg-gray-200" />
        <div className="h-7 w-24 rounded-lg bg-gray-200" />
      </div>
    </div>
  )
}

// ─── page ─────────────────────────────────────────────────────────────────────

export default function AddressesPage() {
  const router = useRouter()
  const { isAuthenticated } = useAuthStore()
  const qc = useQueryClient()

  const [modalOpen, setModalOpen] = useState(false)
  const [editingAddress, setEditingAddress] = useState<CustomerAddress | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<string | null>(null)
  const [toast, setToast] = useState<ToastProps | null>(null)

  useEffect(() => {
    if (!isAuthenticated) router.push('/login')
  }, [isAuthenticated, router])

  const { data: addresses, isLoading, isError } = useQuery({
    queryKey: ['addresses'],
    queryFn: addressService.getAddresses,
    enabled: isAuthenticated,
  })

  const deleteMutation = useMutation({
    mutationFn: addressService.deleteAddress,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['addresses'] })
      setDeleteTarget(null)
      showToast('Address deleted.', 'success')
    },
    onError: () => {
      showToast('Failed to delete address. Please try again.', 'error')
    },
  })

  const setDefaultMutation = useMutation({
    mutationFn: addressService.setDefault,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['addresses'] })
      showToast('Default address updated.', 'success')
    },
    onError: () => {
      showToast('Failed to update default address.', 'error')
    },
  })

  function showToast(message: string, type: 'success' | 'error') {
    setToast({ message, type })
    setTimeout(() => setToast(null), 3000)
  }

  const openAdd = () => {
    setEditingAddress(null)
    setModalOpen(true)
  }

  const openEdit = (address: CustomerAddress) => {
    setEditingAddress(address)
    setModalOpen(true)
  }

  const closeModal = () => {
    setModalOpen(false)
    setEditingAddress(null)
  }

  const handleSaved = (message: string) => {
    closeModal()
    showToast(message, 'success')
  }

  if (!isAuthenticated) return null

  return (
    <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6 lg:px-8 space-y-6">
      {/* back link */}
      <Link
        href="/account"
        className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-800 transition-colors"
      >
        <ArrowLeft size={15} />
        Back to Account
      </Link>

      {/* page header */}
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">My Addresses</h1>
        <button
          onClick={openAdd}
          className="flex items-center gap-2 rounded-xl bg-primary-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-primary-700 transition-colors shadow-sm"
        >
          <Plus size={16} />
          Add New Address
        </button>
      </div>

      {/* error state */}
      {isError && (
        <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          Failed to load addresses. Please refresh the page and try again.
        </div>
      )}

      {/* loading skeleton — 3 cards */}
      {isLoading && (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <SkeletonCard />
          <SkeletonCard />
          <SkeletonCard />
        </div>
      )}

      {/* empty state */}
      {!isLoading && !isError && (!addresses || addresses.length === 0) && (
        <div className="flex flex-col items-center gap-4 rounded-2xl border-2 border-dashed border-gray-200 py-16 text-center">
          <div className="rounded-full bg-gray-100 p-4">
            <MapPin size={32} className="text-gray-400" />
          </div>
          <div>
            <h2 className="text-base font-semibold text-gray-900">No saved addresses yet.</h2>
            <p className="mt-1 text-sm text-gray-500">
              Add a delivery address to get started.
            </p>
          </div>
          <button
            onClick={openAdd}
            className="flex items-center gap-2 rounded-xl bg-primary-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-primary-700 transition-colors"
          >
            <Plus size={15} />
            Add Your First Address
          </button>
        </div>
      )}

      {/* address grid */}
      {!isLoading && addresses && addresses.length > 0 && (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {addresses.map((address) => (
            <AddressCard
              key={address.id}
              address={address}
              onEdit={openEdit}
              onDelete={(id) => setDeleteTarget(id)}
              onSetDefault={(id) => setDefaultMutation.mutate(id)}
              isSettingDefault={
                setDefaultMutation.isPending &&
                setDefaultMutation.variables === address.id
              }
              isDeleting={
                deleteMutation.isPending &&
                deleteTarget === address.id
              }
            />
          ))}
        </div>
      )}

      {/* add / edit modal */}
      <AddressModal
        open={modalOpen}
        onClose={closeModal}
        editing={editingAddress}
        onSaved={handleSaved}
      />

      {/* confirm delete dialog */}
      <ConfirmDialog
        open={deleteTarget !== null}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={() => {
          if (deleteTarget) deleteMutation.mutate(deleteTarget)
        }}
        loading={deleteMutation.isPending}
      />

      {/* toast notification */}
      {toast && <Toast message={toast.message} type={toast.type} />}
    </div>
  )
}
