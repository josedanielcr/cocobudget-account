using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using web_api.Contracts.User.Responses;
using web_api.Database;
using web_api.Shared;

namespace web_api.Features.User;

public static class IsUserRegistered
{
    
    public class Query : IRequest<Result<IsUserRegisteredResponse>>
    {
        public string Email { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
    
    internal sealed class Handler : IRequestHandler<Query, Result<IsUserRegisteredResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<Query> _validator;

        public Handler(ApplicationDbContext context, IValidator<Query> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Result<IsUserRegisteredResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<IsUserRegisteredResponse>(new Error("IsUserRegistered.Validation", validationResult.ToString()));
            }
            
            var user = await _context
                .Users
                .Where(x => x.Email == request.Email)
                .FirstOrDefaultAsync(cancellationToken);
            
            return user == null ? new IsUserRegisteredResponse(false) : new IsUserRegisteredResponse(true);
        }
    }
}

public class IsUserRegisteredEndpoint : ICarterModule
{
    private const string RouteTag = "Users";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user/is-registered/{email}", async (string email, ISender sender) =>
        {
            var query = new IsUserRegistered.Query
            {
                Email = email
            };
            
            var result = await sender.Send(query);
            return result.IsFailure 
                ? Results.BadRequest(result.Error) 
                : Results.Ok(result);
        }).WithTags(RouteTag);
    }
}