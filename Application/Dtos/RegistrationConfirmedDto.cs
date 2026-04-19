namespace Application.Dtos;

public class RegistrationConfirmedDto
{
    public Guid RegistrationId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
