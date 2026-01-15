using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Application.Queries;

public class GetDashboardMetricsHandler
{
    private readonly IDashboardRepository _repository; // Usa Interface, não DbContext!
    private readonly IDistributedCache _redisCache; 
    private readonly IMemoryCache _memoryCache;

    public GetDashboardMetricsHandler(
        IDashboardRepository repository,
        IDistributedCache redisCache,
        IMemoryCache memoryCache)
    {
        _repository = repository;
        _redisCache = redisCache;
        _memoryCache = memoryCache;
    }

    public async Task<DashboardMetrics> Handle(GetDashboardMetricsQuery query)
    {
        var cacheKey = $"dashboard:metrics:{query.RestaurantId}";

        // 1. Tenta L1 (Memória)
        if (_memoryCache.TryGetValue(cacheKey, out DashboardMetrics? memoryResult) && memoryResult != null)
        {
            return memoryResult;
        }

        // 2. Tenta L2 (Redis)
        var redisData = await _redisCache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(redisData))
        {
            var redisResult = JsonSerializer.Deserialize<DashboardMetrics>(redisData);
            if (redisResult != null)
            {
                _memoryCache.Set(cacheKey, redisResult, TimeSpan.FromMinutes(1));
                return redisResult;
            }
        }

        // 3. Calcula no Banco (Via Repositório)
        var metrics = await _repository.GetMetricsAsync(query.RestaurantId);

        // 4. Salva nos Caches
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) };
        await _redisCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(metrics), options);
        _memoryCache.Set(cacheKey, metrics, TimeSpan.FromMinutes(1));

        return metrics;
    }
}
