using Application.Common;
using Domain.Entities;

namespace Application.Repositories;

public interface IRegistrationRepository
{
    public  Task<Result<Registration>> SaveUser(Registration model);
    public Task<string?> GetUserPassword(string userName);
    public Task<Registration?> GetUserName(string password);
    public Task<Result<Registration>> Delete(string userName);
    public Task<bool> AuthorizeUser(string password, string userName);
    public Task<bool> IsUserExists(string userName);
    public Task<Registration?> ChangePassword(string UserName, string NewPassword);
    public Task<List<Registration>> GetAllUsers();
    public Task<bool> Confirm(string password, string name);
}