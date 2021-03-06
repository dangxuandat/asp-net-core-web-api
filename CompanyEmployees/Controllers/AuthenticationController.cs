using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace CompanyEmployees.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public AuthenticationController (ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        //[HttpPost]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        //{
        //    var user = _mapper.Map<User>(userForRegistration);
            
        //    if (!result.Succeeded)
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.TryAddModelError(error.Code, error.Description);
        //        }

        //        return BadRequest(ModelState);
        //    }

           
        //    return StatusCode(201);
        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        //{
        //    if (!await _authenticationManager.ValidateUser(user))
        //    {
        //        _logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password");
        //        return Unauthorized();
        //    }

        //    return Ok(new { Token = await _authenticationManager.CreateToken() });
        //}
    }
}
