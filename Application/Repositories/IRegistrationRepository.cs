using Application.Common;
using Domain.Entities;

namespace Application.Repositories;

public interface IRegistrationRepository
{
    public  Task<Result<Registration>> SaveUser(Registration model);
    public Task<Registration?> GetUserPassword(string userName);
    public Task<Registration?> GetUserName(string password);
    public Task<Result<Registration>> Delete(string userName);
    
}