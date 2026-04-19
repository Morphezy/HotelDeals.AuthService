using Application.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints;

[Controller]
public class Api(ILogger<Api> logger, IRegistrationRepository registrationRepository,
    IUsersRepository usersRepository, ITokenService tokenService, AuthDbContext context) : ControllerBase
{
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
            var model = new Registration() { Password = pass, UserName = userName };
            var res = await _registrationRepository.SaveUser(model);
            return res.isSuccess ? Ok(res.Value.Password) : BadRequest(res.Error);
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

    [HttpGet("/Auth/Login")]
    public async Task<IActionResult> AddUser([FromBody]UserDto dto)
    {
        var res = await _usersRepository.SaveUser(dto.userName, dto.password);
        return res.isSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }

    public async Task<IActionResult> GetUser(string userName)
    {
        var user = await _usersRepository.GetUser(userName);
        return user is null ? NotFound() : Ok(user);
        
    }

    [HttpPost("/Auth/Authorize")]
    public async Task<IActionResult> Authorize([FromBody]UserDto dto)
    {
        var res = await _registrationRepository.AuthorizeUser(dto.password, dto.userName);
        if (!res)
        {
            return Unauthorized("Invalid credentials");
        }

        var token = await _tokenService.GenerateToken(dto.userName);
        await _usersRepository.SaveUser(dto.userName, token);
        return Ok();
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
