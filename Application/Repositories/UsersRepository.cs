using Application.Common;
using Domain.Entities;

namespace Application.Repositories;

public interface IUsersRepository
{
    public Task<Result<User>> SaveUser(string userName, string password);
    public Task<User?> GetUser(string userName);
    public Task<Result<User>> Delete(string userName);
    public Task<bool> GetUserByPassword(string password);
}