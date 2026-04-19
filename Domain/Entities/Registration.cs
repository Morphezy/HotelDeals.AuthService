namespace Domain.Entities;

public class Registration
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

   
}
