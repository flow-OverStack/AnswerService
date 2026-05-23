using AnswerService.Cache.Helpers;
using AnswerService.Cache.Interfaces;
using AnswerService.Cache.Repositories.Base;
using AnswerService.Cache.Settings;
using AnswerService.Domain.Dto.Vote;
using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Provider;
using AnswerService.Domain.Interfaces.Repository.Cache;
using Microsoft.Extensions.Options;

namespace AnswerService.Cache.Repositories;

public class VoteCacheRepository : IVoteCacheRepository
{
    private const string VoteValuePattern = "{0},{1}";

    private readonly IBaseCacheRepository<Vote, VoteDto> _repository;

    public VoteCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Vote, VoteDto>(
            cacheProvider,
            new VoteCacheMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<Vote>> GetByDtosAsync(IEnumerable<VoteDto> dtos,
        Func<IEnumerable<VoteDto>, CancellationToken, Task<IEnumerable<Vote>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(dtos, fetch, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetAnswersVotesAsync(
        IEnumerable<long> answerIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            answerIds,
            CacheKeyHelper.GetAnswerKey,
            CacheKeyHelper.GetAnswerVotesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserVotesKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserVotesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(
        IEnumerable<long> voteTypeIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            voteTypeIds,
            CacheKeyHelper.GetVoteTypeKey,
            CacheKeyHelper.GetVoteTypeVotesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    private sealed class VoteCacheMapping : ICacheEntityMapping<Vote, VoteDto>
    {
        public VoteDto GetId(Vote entity)
        {
            return new VoteDto(entity.AnswerId, entity.UserId);
        }

        public string GetKey(VoteDto id)
        {
            return CacheKeyHelper.GetVoteKey(id.AnswerId, id.UserId);
        }

        public string GetValue(Vote entity)
        {
            return string.Format(VoteValuePattern, entity.AnswerId, entity.UserId);
        }

        public VoteDto ParseIdFromKey(string key)
        {
            var ex = new ArgumentException($"Invalid key format: {key}");

            var parts = key.Split(':');
            if (parts.Length < 2)
                throw ex;

            parts = parts[1].Split(',');
            if (parts.Length < 2)
                throw ex;

            var ids = parts.Select(long.Parse).ToArray();
            return new VoteDto(ids[0], ids[1]);
        }

        public VoteDto ParseIdFromValue(string value)
        {
            var parts = value.Split(',');
            if (parts.Length < 2)
                throw new ArgumentException($"Invalid value format: {value}");

            var ids = parts.Select(long.Parse).ToArray();

            return new VoteDto(ids[0], ids[1]);
        }
    }
}