using AnswerService.Domain.Entities;

namespace AnswerService.Domain.Interfaces.Repository.Cache;

public interface IAnswerCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of answers from the cache by their identifiers.
    ///     If any answers are missing, they are fetched using <paramref name="fetch" /> and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the answers to retrieve.</param>
    /// <param name="fetch"> A fallback delegate that fetches missing answers by their identifiers.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Answer}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<Answer>> GetByIdsAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Answer>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves answers grouped by user identifiers from the cache.
    ///     If any user or answers are missing, the data is fetched using <paramref name="fetch" /> and cached accordingly.
    /// </summary>
    /// <param name="userIds">The list of user IDs whose answers should be retrieved.</param>
    /// <param name="fetch">A fallback delegate that fetches answers grouped by user ids.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Answer}" /> containing a lookup-like list of user Ids to the list of answers.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>> GetUsersAnswersAsync(IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves answers grouped by question identifiers from the cache.
    ///     If any question or answers are missing, the data is fetched using <paramref name="fetch" /> and cached accordingly.
    /// </summary>
    /// <param name="questionIds">The list of question IDs whose answers should be retrieved.</param>
    /// <param name="fetch">A fallback delegate that fetches answers grouped by question ids.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Answer}" /> containing a lookup-like list of question Ids to the list of answers.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>> GetQuestionsAnswersAsync(IEnumerable<long> questionIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Answer>>>>> fetch,
        CancellationToken cancellationToken = default);
}