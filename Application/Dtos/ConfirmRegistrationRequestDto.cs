namespace Application.Dtos;

public class ConfirmRegistrationRequestDto
{
    public long TelegramUserId { get; set; }
    public string Code { get; set; } = string.Empty;
}
