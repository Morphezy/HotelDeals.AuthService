using Application.Common;
using Domain.Entities;

namespace Application.Repositories;

public interface IUsersRepository
{
    public Task<Result<User>> SaveUser(string userName, string token, string telegramId);
    public Task<User?> GetUser(string userName);
    public Task<Result<User>> Delete(string userName);
    public Task<bool> AuthorizeUser(string password, string userName);
    public Task<List<User>> GetUsers();
}