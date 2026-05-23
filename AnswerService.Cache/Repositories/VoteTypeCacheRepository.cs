using AnswerService.Cache.Helpers;
using AnswerService.Cache.Interfaces;
using AnswerService.Cache.Repositories.Base;
using AnswerService.Cache.Settings;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository.Cache;
using Microsoft.Extensions.Options;

namespace AnswerService.Cache.Repositories;

public class VoteTypeCacheRepository : IVoteTypeCacheRepository
{
    private readonly IBaseCacheRepository<VoteType, long> _repository;

    public VoteTypeCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<VoteType, long>(
            cacheProvider,
            new CacheVoteTypeMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<VoteType>> GetByIdsAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<VoteType>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, cancellationToken);
    }

    private sealed class CacheVoteTypeMapping : ICacheEntityMapping<VoteType, long>
    {
        public long GetId(VoteType entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetVoteTypeKey(id);
        }

        public string GetValue(VoteType entity)
        {
            return entity.Id.ToString();
        }

        public long ParseIdFromKey(string key)
        {
            return CacheKeyHelper.GetIdFromKey(key);
        }

        public long ParseIdFromValue(string value)
        {
            return long.Parse(value);
        }
    }
}