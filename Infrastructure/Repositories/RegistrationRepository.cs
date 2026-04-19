using Application.Common;
using Application.Repositories;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class RegistrationRepository(AuthDbContext context, ILogger<RegistrationRepository> logger) : IRegistrationRepository
{
    private readonly AuthDbContext _context = context;
    private readonly ILogger<RegistrationRepository> _logger = logger;
   
    public async Task<Result<Registration>> SaveUser(Registration model)
    {
        try
        {
            await _context.Registrations.AddAsync(model);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Registration model created successfully");
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create registration model");
            return new Error("RegistrationSaveFailed", ErrorType.Validation, ex.Message);
        }
    }

    public async Task<string?> GetUserPassword(string userName)
    {
        var model =await _context.Registrations
            .FirstOrDefaultAsync(x => x.UserName == userName);
        return model.Password;
    }

    public async Task<Registration?> GetUserName(string password)
    {
        return await _context.Registrations
            .FirstOrDefaultAsync(x => x.Password == password);
    }

    public async Task<Result<Registration>> Delete(string userName)
    {
        var registration = await _context.Registrations
            .FirstOrDefaultAsync(x => x.UserName == userName);

        if (registration is null)
        {
            return Errors.AccountNotFound;
        }

        try
        {
            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Registration model deleted successfully");
            return registration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete registration model");
            return new Error("RegistrationDeleteFailed", ErrorType.Validation, ex.Message);
        }
    }

    public async Task<bool> AuthorizeUser(string password, string userName)
    {
        var user = await _context.Registrations
            .FirstOrDefaultAsync(x => x.UserName == userName);
        return user?.Password == password;
    }

    public async Task<bool> IsUserExists(string userName)
    {
        var user = await _context.Registrations.FirstOrDefaultAsync(x => x.UserName == userName);
        return  user != null;
    }

    public async Task<Registration> ChangePassword(string UserName, string NewPassword)
    {
        var user = await _context.Registrations.FirstOrDefaultAsync(a => a.UserName == UserName);
        user.Password = NewPassword;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<List<Registration>> GetAllUsers()
    {
        return await _context.Registrations.ToListAsync();
    }
}
