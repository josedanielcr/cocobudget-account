using System.ComponentModel.DataAnnotations;

namespace web_api.Contracts.Test;

public class CreateTestRequest
{
    [MaxLength(128)] 
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; } = 0;
}