using System.ComponentModel.DataAnnotations;
using Carter;
using FluentValidation;
using MediatR;
using web_api.Contracts.Test;
using web_api.Database;
using web_api.Shared;

namespace web_api.Features.Test;

public static class CreateTest
{
    public class Command : IRequest<Result<Entities.Test>>
    {
        [MaxLength(128)] 
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Age).GreaterThan(18);
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Entities.Test>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }
        
        public async Task<Result<Entities.Test>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<Entities.Test>(new Error("CreateTest.Validation", validationResult.ToString()));
            }
            
            var test = new Entities.Test()
            {
                Name = request.Name,
                Age = request.Age
            };
            
            _dbContext.Tests.Add(test);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return test;
        }
    }
}

public class CreateTestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/test", async (CreateTestRequest request, ISender sender) =>
        {
            var command = new CreateTest.Command
            {
                Name = request.Name,
                Age = request.Age
            };
            
           var result = await sender.Send(command);
           return result.IsFailure 
               ? Results.BadRequest(result.Error) 
               : Results.Ok(result);
        });
    }
}