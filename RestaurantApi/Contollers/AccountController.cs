using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Entities;
using RestaurantApi.IService;
using RestaurantApi.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestaurantApi.Contollers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _acconntService;
        

        public AccountController(IAccountService accountService)
        {
            _acconntService = accountService;
        }
        [HttpPost("register")]
        public ActionResult RegisterAccount([FromBody]RegisterUserDto dto)
        {
            _acconntService.RegisterUser(dto);
            return Ok();
        }
        [HttpPost]
        public ActionResult Login([FromBody]LoginDto dto)
        {
            string token = _acconntService.GenerateJwt(dto);
            return Ok(token);
        }
    }
}

