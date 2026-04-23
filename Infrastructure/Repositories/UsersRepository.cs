using Application.Common;
using Application.Repositories;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class UsersRepository(AuthDbContext context, ILogger<UsersRepository> logger) : IUsersRepository
{
    private readonly AuthDbContext _context = context;
    private readonly ILogger<UsersRepository> _logger = logger;

    public async Task<Result<User>> SaveUser(string userName, string token, string telegramId)
    {
        var user = new User
        {
            UserName = userName,
            Token = token,
            TelegramId = telegramId
        };
        _logger.LogInformation($"user with name  {user.UserName} and token {user.Token}, telegramid is {user.TelegramId}");
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User created successfully");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user");
            return new Error("UserSaveFailed", ErrorType.Validation, ex.Message);
        }
    }

    public async Task<User?> GetUser(string userName)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == userName);
    }

    public async Task<Result<User>> Delete(string userName)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == userName);

        if (user is null)
        {
            return Errors.AccountNotFound;
        }

        try
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User deleted successfully");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user");
            return new Error("UserDeleteFailed", ErrorType.Validation, ex.Message);
        }
    }

    public Task<bool> AuthorizeUser(string password, string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }
}
