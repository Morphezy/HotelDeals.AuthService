namespace Domain.Entities;

public class Registration
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}