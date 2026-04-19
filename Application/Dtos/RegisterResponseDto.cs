namespace Application.Dtos;

public class RegisterResponseDto
{
    public Guid RegistrationId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
