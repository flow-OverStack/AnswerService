using AnswerService.Application.Enums;
using AnswerService.Application.Extensions;
using AnswerService.Application.Queries.VoteType;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.VoteType;

public class GetVoteTypesHandler(IBaseRepository<Domain.Entities.VoteType> voteTypeRepository)
    : IRequestHandler<GetVoteTypesQuery, CollectionResult<Domain.Entities.VoteType>>
{
    public async Task<CollectionResult<Domain.Entities.VoteType>> Handle(GetVoteTypesQuery request,
        CancellationToken cancellationToken)
    {
        var voteTypeIds = request.VoteTypeIds.ToArray();
        var voteTypes = await voteTypeRepository.GetAll()
            .Where(x => voteTypeIds.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (voteTypes.Length == 0)
            return CollectionResult<Domain.Entities.VoteType>.VoteTypesNotFound(voteTypeIds.Length);

        return CollectionResult<Domain.Entities.VoteType>.Success(voteTypes);
    }
}