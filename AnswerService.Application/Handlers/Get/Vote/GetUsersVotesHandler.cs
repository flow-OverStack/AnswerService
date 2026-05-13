using AnswerService.Application.Commands.GetCommands.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetUsersVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetUsersVotesCommand, CollectionResult<Domain.Entities.Vote>>
{
    public async Task<CollectionResult<Domain.Entities.Vote>> Handle(GetUsersVotesCommand request,
        CancellationToken cancellationToken)
    {
        var userIds = request.UserIds.ToArray();
        var votes = await voteRepository.GetAll()
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}