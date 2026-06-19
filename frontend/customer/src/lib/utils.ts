import { clsx, type ClassValue } from 'clsx'
import { twMerge } from 'tailwind-merge'

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatBDT(amount: number): string {
  return `৳${amount.toLocaleString('en-BD', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`
}

export function formatDate(date: string | Date): string {
  return new Date(date).toLocaleDateString('en-BD', { year: 'numeric', month: 'short', day: 'numeric' })
}

export function slugify(text: string): string {
  return text.toLowerCase().replace(/[^\w\s-]/g, '').replace(/[\s_-]+/g, '-').trim()
}

export function truncate(text: string, length = 100): string {
  return text.length > length ? text.slice(0, length) + '…' : text
}
