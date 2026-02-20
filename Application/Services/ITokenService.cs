namespace Application.Services;

public interface ITokenService
{
    public  Task<string> GenerateToken(string userName);
}