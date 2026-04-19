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

    public async Task<Result<Registration>> CreateOrUpdatePending(string userName, long telegramUserId, string code, DateTime expiresAtUtc)
    {
        var registration = await _context.Registrations
            .FirstOrDefaultAsync(x => x.UserName == userName || x.TelegramUserId == telegramUserId);

        if (registration is null)
        {
            registration = new Registration
            {
                UserName = userName,
                TelegramUserId = telegramUserId,
                Code = code,
                Status = "Pending",
                ExpiresAtUtc = expiresAtUtc
            };

            try
            {
                await _context.Registrations.AddAsync(registration);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pending registration created for {UserName}", userName);
                return registration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create pending registration for {UserName}", userName);
                return new Error("RegistrationSaveFailed", ErrorType.Validation, ex.Message);
            }
        }

        try
        {
            registration.UserName = userName;
            registration.TelegramUserId = telegramUserId;
            registration.Code = code;
            registration.Status = "Pending";
            registration.ExpiresAtUtc = expiresAtUtc;
            registration.ConfirmedAtUtc = null;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Pending registration updated for {UserName}", userName);
            return registration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update pending registration for {UserName}", userName);
            return new Error("RegistrationSaveFailed", ErrorType.Validation, ex.Message);
        }
    }

    public async Task<Registration?> GetById(Guid registrationId)
    {
        return await _context.Registrations
            .FirstOrDefaultAsync(x => x.Id == registrationId);
    }

    public async Task<Result<Registration>> Confirm(long telegramUserId, string code)
    {
        var registration = await _context.Registrations
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);

        if (registration is null)
        {
            return Errors.AccountNotFound;
        }

        if (!string.Equals(registration.Status, "Pending", StringComparison.Ordinal))
        {
            return new Error("RegistrationAlreadyProcessed", ErrorType.Validation, "Registration is not pending.");
        }

        if (registration.ExpiresAtUtc < DateTime.UtcNow)
        {
            return new Error("RegistrationCodeExpired", ErrorType.Validation, "Registration code has expired.");
        }

        if (!string.Equals(registration.Code, code, StringComparison.Ordinal))
        {
            return new Error("RegistrationCodeInvalid", ErrorType.Validation, "Registration code is invalid.");
        }

        try
        {
            registration.Status = "Confirmed";
            registration.ConfirmedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Pending registration confirmed for {UserName}", registration.UserName);
            return registration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm pending registration for Telegram user {TelegramUserId}", telegramUserId);
            return new Error("RegistrationConfirmFailed", ErrorType.Validation, ex.Message);
        }
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

    public async Task<List<Registration>> GetAllUsers()
    {
        return await _context.Registrations.ToListAsync();
    }
}
