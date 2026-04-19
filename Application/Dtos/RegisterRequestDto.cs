namespace Application.Dtos;

public class RegisterRequestDto
{
    public string UserName { get; set; } = string.Empty;
    public long TelegramUserId { get; set; }
}
