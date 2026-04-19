namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
