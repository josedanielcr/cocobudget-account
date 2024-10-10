using Carter;
using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using web_api.Contracts.User.Requests;
using web_api.Database;
using web_api.Shared;

namespace web_api.Features.User
{

    public static class UpdateUser
    {
        public class Command : IRequest<Result>
        {
            public Guid Id { get; set; }

            [MaxLength(128)]
            public string FirstName { get; set; } = null!;

            [MaxLength(128)]
            public string LastName { get; set; } = null!;

            [EmailAddress]
            public string Email { get; set; } = null!;

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

        internal sealed class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _dbContext;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(new Error("UpdateUser.Validation", validationResult.ToString()));
                }

                var user = await _dbContext.Users.FindAsync(request.Id);

                if (user == null)
                {
                    return Result.Failure(new Error("User.NotFound", $"User with ID {request.Id} not found."));
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.ProfilePicture = request.ProfilePicture ?? string.Empty;

                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }

    public class UpdateUserEndpoint : ICarterModule
    {
        private const string RouteTag = "Users";

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/users/{id:guid}", async (Guid id, UpdateUserRequest request, ISender sender) =>
            {
                var command = new UpdateUser.Command
                {
                    Id = id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    ProfilePicture = request.ProfilePicture
                };

                var result = await sender.Send(command);
                return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
            }).WithTags(RouteTag);
        }
    }
}
