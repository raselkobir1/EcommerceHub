import { useQuery } from '@tanstack/react-query'
import { bannerService } from '@/services/bannerService'
import Badge from '@/components/ui/Badge'

export default function BannersPage() {
  const { data, isLoading } = useQuery({
    queryKey: ['banners'],
    queryFn: () => bannerService.getBanners(),
  })

  const banners = data?.items ?? []

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <button className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 transition-colors">
          + Add Banner
        </button>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {isLoading && Array.from({ length: 3 }).map((_, i) => (
          <div key={i} className="h-48 rounded-xl bg-gray-200 animate-pulse" />
        ))}
        {!isLoading && banners.map((banner) => (
          <div key={banner.id} className="bg-white rounded-xl border overflow-hidden">
            <div className="relative h-36 bg-gray-100">
              <img src={banner.imageUrl} alt={banner.title} className="h-full w-full object-cover" />
              <div className="absolute top-2 right-2">
                <Badge variant={banner.isActive ? 'success' : 'default'}>
                  {banner.isActive ? 'Active' : 'Inactive'}
                </Badge>
              </div>
            </div>
            <div className="p-3">
              <p className="font-medium text-gray-900">{banner.title}</p>
              {banner.linkUrl && <p className="text-xs text-gray-400 truncate mt-0.5">{banner.linkUrl}</p>}
            </div>
          </div>
        ))}
        {!isLoading && banners.length === 0 && (
          <div className="col-span-3 py-12 text-center text-gray-400">No banners configured</div>
        )}
      </div>
    </div>
  )
}
