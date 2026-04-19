namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string TelegramId { get; set; } = string.Empty;
    public string UserName { get; set; }
    public string Token { get; set; }
}