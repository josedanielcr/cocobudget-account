using System.ComponentModel.DataAnnotations;
using web_api.Entities;

namespace web_api.Contracts.User.Responses;

public class UserResponse : BaseEntity
{
    public UserResponse(Guid id, string firstName, string lastName, string email, bool isVerified, string profilePicture, bool isActive, 
        DateTime createdOn, DateTime modifiedOn, int budgetDayLength)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IsVerified = isVerified;
        ProfilePicture = profilePicture;
        IsActive = isActive;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
        BudgetDayLength = budgetDayLength;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsVerified { get; set; }
    public string ProfilePicture { get; set; }
    public required int BudgetDayLength { get; set; }
}