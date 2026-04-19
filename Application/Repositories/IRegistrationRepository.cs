using Application.Common;
using Domain.Entities;

namespace Application.Repositories;

public interface IRegistrationRepository
{
    public Task<Result<Registration>> CreateOrUpdatePending(string userName, long telegramUserId, string code, DateTime expiresAtUtc);
    public Task<Registration?> GetById(Guid registrationId);
    public Task<Result<Registration>> Confirm(long telegramUserId, string code);
    public Task<Result<Registration>> Delete(string userName);
    public Task<List<Registration>> GetAllUsers();
}
