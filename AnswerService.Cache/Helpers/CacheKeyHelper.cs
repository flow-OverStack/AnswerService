namespace AnswerService.Cache.Helpers;

public static class CacheKeyHelper
{
    private const string AnswerKeyPattern = "answer:{0}";
    private const string UserAnswersKeyPattern = "user:{0}:answers";
    private const string QuestionAnswersKeyPattern = "question:{0}:answers";

    private const string VoteKeyPattern = "vote:{0},{1}";
    private const string AnswerVotesKeyPattern = "answer:{0}:votes";
    private const string UserVotesKeyPattern = "user:{0}:votes";
    private const string VoteTypeVotesKeyPattern = "voteType:{0}:votes";

    private const string VoteTypeKeyPattern = "voteType:{0}";

    public static string GetAnswerKey(long id)
    {
        return string.Format(AnswerKeyPattern, id);
    }

    public static string GetUserAnswersKey(long userId)
    {
        return string.Format(UserAnswersKeyPattern, userId);
    }

    public static string GetQuestionAnswersKey(long questionId)
    {
        return string.Format(QuestionAnswersKeyPattern, questionId);
    }

    public static string GetVoteKey(long answerId, long userId)
    {
        return string.Format(VoteKeyPattern, answerId, userId);
    }

    public static string GetAnswerVotesKey(long answerId)
    {
        return string.Format(AnswerVotesKeyPattern, answerId);
    }

    public static string GetUserVotesKey(long userId)
    {
        return string.Format(UserVotesKeyPattern, userId);
    }

    public static string GetVoteTypeVotesKey(long voteType)
    {
        return string.Format(VoteTypeVotesKeyPattern, voteType);
    }

    public static string GetVoteTypeKey(long id)
    {
        return string.Format(VoteTypeKeyPattern, id);
    }

    public static long GetIdFromKey(string key)
    {
        var parts = key.Split(':');

        var ex = new ArgumentException($"Invalid key format: {key}");
        return parts.Length switch
        {
            2 => long.Parse(parts[1]),
            3 => TryParseLong(parts[1]) ??
                 TryParseLong(parts[2]) ?? throw ex,
            _ => throw ex
        };
    }

    private static long? TryParseLong(string str)
    {
        return long.TryParse(str, out var result) ? result : null;
    }
}