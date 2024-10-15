using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using web_api.Contracts.User.Requests;
using web_api.Database;
using web_api.Shared;

namespace web_api.Features.Users;

public static class GetUser
{
    public class Query : IRequest<Result<Entities.User>>
    {
        [EmailAddress]
        public required string Email { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Entities.User>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Entities.User>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            return user != null ? user : Result.Failure<Entities.User>(new Error("User.NotFound", $"User with email {request.Email} not found."));
        }
    }
}

public class GetUserEndpoint : ICarterModule
{
    private const string RouteTag = "Users";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/users/{email}", async (string email, ISender sender) =>
        {
            var query = new GetUser.Query { Email = email };
            var result = await sender.Send(query);
            return result.IsFailure ? Results.NotFound(result.Error) : Results.Ok(result.Value);
        }).WithTags(RouteTag);
    }
}