using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using web_api.Contracts.Test.Responses;
using web_api.Database;
using web_api.Shared;

namespace web_api.Features.Test;

public static class GetTest
{
    public class Query : IRequest<Result<TestResponse>>
    {
        public Guid Id { get; set; }
    }
    
    internal sealed class Handler : IRequestHandler<Query, Result<TestResponse>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Result<TestResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var test = await _dbContext
                .Tests
                .Where(x => x.Id == request.Id)
                .Select(test => new TestResponse
                {
                    Id = test.Id,
                    Name = test.Name,
                    Age = test.Age,
                    CreatedOn = test.CreatedOn,
                    ModifiedOn = test.ModifiedOn
                })
                .FirstOrDefaultAsync(cancellationToken);
            return test 
                   ?? Result.Failure<TestResponse>(new Error("GetTest.Null","The test with the given id was not found"));
        }
    }
}

public class GetTestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/test/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetTest.Query
            {
                Id = id
            };
            var result = await sender.Send(query);
            return result.IsFailure 
                ? Results.NotFound(result.Error) 
                : Results.Ok(result.Value);
        });
    }
}