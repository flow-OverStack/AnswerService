using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DefaultVoteTypesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 INSERT INTO public."VoteType"("Name", "MinReputationToVote", "ReputationChange") 
                                 VALUES ('Upvote', 15, 1), ('Downvote', 125, -1);
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 DELETE FROM public."VoteType"
                                 WHERE "Name" IN ('Upvote', 'Downvote');
                                 """);
        }
    }
}
