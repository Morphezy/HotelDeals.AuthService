using Application.Repositories;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints;

[Controller]
public class Api(ILogger<Api> logger, IRegistrationRepository registrationRepository,
    IUsersRepository usersRepository) : ControllerBase
{
    ILogger<Api> _logger = logger;
    IRegistrationRepository _registrationRepository = registrationRepository;
    IUsersRepository _usersRepository = usersRepository;
    
    
    [HttpGet("/Auth/Register")]
    public async Task<IActionResult> SaveToReg(string  userName)
    {
     var pass = RandomStringGenService.RandomString(6);
     if (pass is null)
     {
         return BadRequest("try again");
     }
     var model = new Registration() { Password = pass, UserName = userName }; 
     var res =  await _registrationRepository.SaveUser(model);
     return  res.isSuccess? Ok(res.Value) : BadRequest(res.Error);

    }

    [HttpGet("/Auth/RegisterDelete")]
    public async Task<IActionResult> DeleteFromReg(string name)
    {
        var res = await _registrationRepository.Delete(name);
        return res.isSuccess ? Ok(res.Value) : BadRequest(res.Error);
    }
    
    
}