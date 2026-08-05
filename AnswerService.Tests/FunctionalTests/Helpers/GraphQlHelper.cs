namespace AnswerService.Tests.FunctionalTests.Helpers;

internal static class GraphQlHelper
{
    public const string GraphQlEndpoint = "/graphql";

    public const string RequestAllQuery = """
                                          {
                                            answers(first: 5, order: [{ lastModifiedAt: DESC }]) {
                                              edges {
                                                cursor
                                                node {
                                                  id
                                                  body
                                                  createdAt
                                                  lastModifiedAt
                                                  questionId
                                                  isAccepted
                                                  reputation
                                                  votes {
                                                    userId
                                                    voteType {
                                                      minReputationToVote
                                                      reputationChange
                                                      name
                                                      votes {
                                                        userId
                                                        answerId
                                                      }
                                                    }
                                                  }
                                                }
                                              }
                                              totalCount
                                              pageInfo {
                                                hasNextPage
                                                hasPreviousPage
                                                startCursor
                                                endCursor
                                              }
                                            }
                                            answerVotes(take: 10) {
                                              items {
                                                userId
                                                answerId
                                                voteType {
                                                  name
                                                  reputationChange
                                                  minReputationToVote
                                                }
                                              }
                                              totalCount
                                            }
                                            answerVoteTypes(take: 10) {
                                              items {
                                                name
                                                reputationChange
                                                minReputationToVote
                                                votes {
                                                  userId
                                                  answerId
                                                }
                                              }
                                              totalCount
                                            }
                                          }
                                          """;

    public const string RequestWithWrongArgument = """
                                                   answer(wrongArg){
                                                     body
                                                     createdAt
                                                     lastModifiedAt
                                                     questionId
                                                     reputation
                                                     isAccepted
                                                   }
                                                   """;

    public const string RequestWithInvalidPaginationQuery = """
                                                            {
                                                              answers(after: "e30x", first: -1, order: [{ lastModifiedAt: DESC }]) {
                                                                edges {
                                                                  cursor
                                                                  node {
                                                                    id
                                                                    body
                                                                    createdAt
                                                                    lastModifiedAt
                                                                    questionId
                                                                    isAccepted
                                                                    reputation
                                                                    votes {
                                                                      userId
                                                                      voteType {
                                                                        minReputationToVote
                                                                        reputationChange
                                                                        name
                                                                        votes {
                                                                          userId
                                                                          answerId
                                                                        }
                                                                      }
                                                                    }
                                                                  }
                                                                }
                                                                totalCount
                                                                pageInfo {
                                                                  hasNextPage
                                                                  hasPreviousPage
                                                                  startCursor
                                                                  endCursor
                                                                }
                                                              }
                                                              answerVotes(take: 201) {
                                                                items {
                                                                  userId
                                                                  answerId
                                                                  voteType {
                                                                    name
                                                                    reputationChange
                                                                    minReputationToVote
                                                                  }
                                                                }
                                                              }
                                                              answerVoteTypes(take: -1) {
                                                                items {
                                                                  name
                                                                  reputationChange
                                                                  minReputationToVote
                                                                  votes {
                                                                    userId
                                                                    answerId
                                                                  }
                                                                }
                                                              }
                                                            }
                                                            """;

    public static string RequestAllByIdsQuery(long answerId, long voteAnswerId, long voteUserId, long voteTypeId)
    {
        return """
               query {
                 answer(id: $ANSWERID) {
                   body
                   createdAt
                   lastModifiedAt
                   questionId
                   isAccepted
                   reputation
                   votes {
                     userId
                     voteType {
                       minReputationToVote
                       reputationChange
                       name
                       votes {
                         userId
                         answerId
                       }
                     }
                   }
                 }
                 answerVote(userId: $VOTEUSERID, answerId: $VOTEANSWERID) {
                   voteType {
                     name
                     reputationChange
                     votes {
                       userId
                       answerId
                     }
                   }
                 }
                 answerVoteType(id: $VOTETYPEID) {
                   minReputationToVote
                   name
                   reputationChange
                   votes {
                     userId
                     answerId
                   }
                 }
               }
               """
            .Replace("$ANSWERID", answerId.ToString())
            .Replace("$VOTEANSWERID", voteAnswerId.ToString())
            .Replace("$VOTEUSERID", voteUserId.ToString())
            .Replace("$VOTETYPEID", voteTypeId.ToString());
    }

    public static string RequestAnswerByIdQuery(long answerId)
    {
        return """
               query {
                 answer(id: $ANSWERID) {
                   body
                   createdAt
                   lastModifiedAt
                   questionId
                   isAccepted
                   reputation
                   votes {
                     userId
                     voteType {
                       minReputationToVote
                       reputationChange
                       name
                       votes {
                         userId
                         answerId
                       }
                     }
                   }
                 }
               }
               """
            .Replace("$ANSWERID", answerId.ToString());
    }
}