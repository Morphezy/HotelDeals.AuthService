namespace Domain.Entities;

public class Registration
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public long TelegramUserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConfirmedAtUtc { get; set; }
}
