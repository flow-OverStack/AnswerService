using AnswerService.Cache.Helpers;
using AnswerService.Cache.Interfaces;
using AnswerService.Cache.Repositories.Base;
using AnswerService.Cache.Settings;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository.Cache;
using Microsoft.Extensions.Options;

namespace AnswerService.Cache.Repositories;

public class AnswerCacheRepository : IAnswerCacheRepository
{
    private readonly IBaseCacheRepository<Answer, long> _repository;

    public AnswerCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Answer, long>(
            cacheProvider,
            new CacheAnswerMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<Answer>> GetByIdsAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Answer>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>> GetUsersAnswersAsync(
        IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserAnswersKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserAnswersKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>> GetQuestionsAnswersAsync(
        IEnumerable<long> questionIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionAnswersKey,
            CacheKeyHelper.GetQuestionAnswersKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    private sealed class CacheAnswerMapping : ICacheEntityMapping<Answer, long>
    {
        public long GetId(Answer entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetAnswerKey(id);
        }

        public string GetValue(Answer entity)
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