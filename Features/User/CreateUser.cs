using System.ComponentModel.DataAnnotations;
using Carter;
using FluentValidation;
using MediatR;
using web_api.Contracts.User.Requests;
using web_api.Database;
using web_api.Shared;

namespace web_api.Features.User;

public class CreateUser
{
    public class Command : IRequest<Result<Entities.User>>
    {
        [MaxLength(128)]
        public required string FirstName { get; set; }

        [MaxLength(128)]
        public required string LastName { get; set; }

        [MaxLength(256)]
        [EmailAddress]
        public required string Email { get; set; }

        [MaxLength(2048)]
        public string? ProfilePicture { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Entities.User>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<Result<Entities.User>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Entities.User>(new Error("CreateUser.Validation", validationResult.ToString()));
            }

            var user = new Entities.User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ProfilePicture = request.ProfilePicture ?? string.Empty
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}

public class CreateUserEndpoint : ICarterModule
{
    private const string RouteTag = "Users";
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/users", async (CreateUserRequest request, ISender sender) =>
        {
            var command = new CreateUser.Command
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ProfilePicture = request.ProfilePicture
            };

            var result = await sender.Send(command);
            return result.IsFailure
                ? Results.BadRequest(result.Error)
                : Results.Ok(result);
        }).WithTags(RouteTag);
    }
}
