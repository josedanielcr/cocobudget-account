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
        public required DateTime BudgetStartDate { get; set; }
        public required DateTime BudgetEndDate { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.BudgetStartDate).NotEmpty();
            RuleFor(x => x.BudgetEndDate).NotEmpty();
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
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Entities.User>(new Error("CreateUser.Validation", validationResult.ToString()));
            }

            var user = new Entities.User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BudgetStartDate = request.BudgetStartDate,
                BudgetEndDate = request.BudgetEndDate
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
        app.MapPost("api/user", async (CreateUserRequest request, ISender sender) =>
        {
            var command = new CreateUser.Command
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BudgetStartDate = request.BudgetStartDate,
                BudgetEndDate = request.BudgetEndDate
            };

            var result = await sender.Send(command);
            return result.IsFailure
                ? Results.BadRequest(result.Error)
                : Results.Ok(result);
        }).WithTags(RouteTag);
    }
}
