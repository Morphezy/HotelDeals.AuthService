using Application.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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

    IHubContext<AuthHub> _hubContext = hubContext;
    AuthDbContext _context = context;
    ILogger<Api> _logger = logger;
    IRegistrationRepository _registrationRepository = registrationRepository;
    IUsersRepository _usersRepository = usersRepository;
    ITokenService _tokenService = tokenService;


    [HttpGet("/Auth/Register")]
    public async Task<IActionResult> SaveToReg(string userName)
    {
        string pass;
        if (await _registrationRepository.IsUserExists(userName))
        {
            pass = await _registrationRepository.GetUserPassword(userName);
            if (pass is null)
            {
                return BadRequest("try again");
            }

            return Ok(pass);
        }
        else
        {
            pass = RandomStringGenService.RandomString(6);
            var model = new Registration() { Code = pass, UserName = userName };
            var res = await _registrationRepository.SaveUser(model);
            return res.isSuccess ? Ok(res.Value.Code) : BadRequest(res.Error);
        }




    }

    [HttpGet("/Auth/Confirm")]
    public async Task<IActionResult> Confirm([FromQuery] ConfirmRegistrationRequestDto dto)
    {
        var res = await _registrationRepository.Confirm(dto.Password, dto.UserName);
        if (res)
        {
            var token = await _tokenService.GenerateToken(dto.UserName);
            await _registrationRepository.Delete(dto.UserName);
            await _usersRepository.SaveUser(dto.UserName, token, dto.TelegramId);


            await _hubContext.Clients.Group(dto.UserName)
                .SendAsync("RegistrationConfirmed", new RegistrationConfirmedDto
                {
                    RegistrationId = new Guid(),
                    UserName = dto.UserName,
                    Token = token
                });
            return Ok();
        }
        else
        {
            return BadRequest(401);

        }
        


    }



[HttpDelete("/Auth/RegisterDelete")]
    public async Task<IActionResult> DeleteFromReg(string name)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try{
            await transaction.CreateSavepointAsync("before");
        var res = await _registrationRepository.Delete(name);
        if (res.isSuccess)
        {
            await transaction.CommitAsync();
            return Ok(res.Value);
        }
        else
        {
            await transaction.RollbackToSavepointAsync("before");
            return BadRequest(res.Error);
        }
        }
        catch(Exception)
        {
          await transaction.RollbackToSavepointAsync("before");  
          return BadRequest("try again");
        }
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
