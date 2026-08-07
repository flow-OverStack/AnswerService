using System.Collections.Concurrent;
using System.Reflection;
using AnswerService.Domain.Results;

namespace AnswerService.Application.Helpers;

public static class ResultFactory
{
    private static readonly ConcurrentDictionary<Type, Func<string, int?, object>> Cache = new();

    public static TResponse Failure<TResponse>(string errorMessage, int? errorCode) where TResponse : BaseResult
    {
        var factory = Cache.GetOrAdd(typeof(TResponse), t =>
        {
            var method = t.GetMethod(nameof(BaseResult.Failure),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)!;
            return (msg, code) => method.Invoke(null, [msg, code])!;
        });
        return (TResponse)factory(errorMessage, errorCode);
    }
}
