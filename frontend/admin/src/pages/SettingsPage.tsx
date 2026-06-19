import { useAuthStore } from '@/stores/authStore'

export default function SettingsPage() {
  const user = useAuthStore((s) => s.user)

  return (
    <div className="max-w-xl space-y-6">
      <div className="bg-white rounded-xl border p-6 space-y-4">
        <h2 className="text-base font-semibold text-gray-900">Profile</h2>
        <div className="space-y-3">
          <div>
            <label className="block text-sm font-medium text-gray-500 mb-1">Full Name</label>
            <input readOnly value={user?.fullName ?? ''} className="w-full rounded-lg border bg-gray-50 px-3 py-2 text-sm text-gray-700 cursor-not-allowed" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-500 mb-1">Email</label>
            <input readOnly value={user?.email ?? ''} className="w-full rounded-lg border bg-gray-50 px-3 py-2 text-sm text-gray-700 cursor-not-allowed" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-500 mb-1">Role</label>
            <input readOnly value={user?.role ?? ''} className="w-full rounded-lg border bg-gray-50 px-3 py-2 text-sm text-gray-700 cursor-not-allowed" />
          </div>
        </div>
      </div>

      <div className="bg-white rounded-xl border p-6 space-y-4">
        <h2 className="text-base font-semibold text-gray-900">Change Password</h2>
        <div className="space-y-3">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Current Password</label>
            <input type="password" className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">New Password</label>
            <input type="password" className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Confirm New Password</label>
            <input type="password" className="w-full rounded-lg border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
          </div>
          <button className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 transition-colors">
            Update Password
          </button>
        </div>
      </div>
    </div>
  )
}
