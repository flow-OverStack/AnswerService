using AnswerService.Application.Queries.VoteType;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Get.VoteType;

public class GetAllVoteTypesHandler(IBaseRepository<Domain.Entities.VoteType> voteTypeRepository)
    : IRequestHandler<GetAllVoteTypesQuery, QueryableResult<Domain.Entities.VoteType>>
{
    public Task<QueryableResult<Domain.Entities.VoteType>> Handle(GetAllVoteTypesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var voteTypes = voteTypeRepository.GetAll();

        return Task.FromResult(QueryableResult<Domain.Entities.VoteType>.Success(voteTypes));
    }
}