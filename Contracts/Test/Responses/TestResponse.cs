using System.ComponentModel.DataAnnotations;

namespace web_api.Contracts.Test.Responses;

public class TestResponse
{
    public Guid? Id { get; set; } = new Guid();
    
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; } = 0;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
}