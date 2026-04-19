using Application.Dtos;
using Application.Repositories;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Endpoints;

[Controller]
public class Api(
    ILogger<Api> logger,
    IRegistrationRepository registrationRepository,
    IUsersRepository usersRepository,
    ITokenService tokenService,
    AuthDbContext context,
    IHubContext<AuthHub> hubContext) : ControllerBase
{
    private readonly AuthDbContext _context = context;
    private readonly ILogger<Api> _logger = logger;
    private readonly IRegistrationRepository _registrationRepository = registrationRepository;
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IHubContext<AuthHub> _hubContext = hubContext;

    [HttpPost("/Auth/Register")]
    public async Task<IActionResult> SaveToReg([FromBody] RegisterRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName))
        {
            return BadRequest("UserName is required.");
        }

        if (dto.TelegramUserId <= 0)
        {
            return BadRequest("TelegramUserId is required.");
        }

        var code = RandomStringGenService.RandomString(6);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(10);
        var result = await _registrationRepository.CreateOrUpdatePending(dto.UserName, dto.TelegramUserId, code, expiresAtUtc);
        if (!result.isSuccess || result.Value is null)
        {
            return BadRequest(result.Error);
        }

        return Ok(new RegisterResponseDto
        {
            RegistrationId = result.Value.Id,
            UserName = result.Value.UserName,
            Code = result.Value.Code,
            ExpiresAtUtc = result.Value.ExpiresAtUtc
        });
    }

    [HttpPost("/Auth/Confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmRegistrationRequestDto dto)
    {
        if (dto.TelegramUserId <= 0)
        {
            return BadRequest("TelegramUserId is required.");
        }

        var confirmResult = await _registrationRepository.Confirm(dto.TelegramUserId, dto.Code);
        if (!confirmResult.isSuccess || confirmResult.Value is null)
        {
            return BadRequest(confirmResult.Error);
        }

        var token = await _tokenService.GenerateToken(confirmResult.Value.UserName);
        var saveUserResult = await _usersRepository.SaveUser(confirmResult.Value.UserName, token);
        if (!saveUserResult.isSuccess)
        {
            return BadRequest(saveUserResult.Error);
        }

        await _hubContext.Clients.Group(confirmResult.Value.Id.ToString())
            .SendAsync("RegistrationConfirmed", new RegistrationConfirmedDto
            {
                RegistrationId = confirmResult.Value.Id,
                UserName = confirmResult.Value.UserName,
                Token = token
            });

        _logger.LogInformation("Registration confirmed and JWT pushed for {UserName}", confirmResult.Value.UserName);
        return Ok();
    }

    [HttpDelete("/Auth/RegisterDelete")]
    public async Task<IActionResult> DeleteFromReg(string name)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await transaction.CreateSavepointAsync("before");
            var res = await _registrationRepository.Delete(name);
            if (res.isSuccess)
            {
                await transaction.CommitAsync();
                return Ok(res.Value);
            }

            await transaction.RollbackToSavepointAsync("before");
            return BadRequest(res.Error);
        }
        catch (Exception)
        {
            await transaction.RollbackToSavepointAsync("before");
            return BadRequest("try again");
        }
    }

    [HttpPost("/Auth/Login")]
    public async Task<IActionResult> AddUser([FromBody] UserDto dto)
    {
        var res = await _usersRepository.SaveUser(dto.userName, dto.password);
        return res.isSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    public async Task<IActionResult> GetUser(string userName)
    {
        var user = await _usersRepository.GetUser(userName);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("/Auth/GetAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _registrationRepository.GetAllUsers());
    }

    [HttpGet("/Auth/GetRegistered")]
    public async Task<IActionResult> GetAllRegistered()
    {
        return Ok(await _usersRepository.GetUsers());
    }
}
