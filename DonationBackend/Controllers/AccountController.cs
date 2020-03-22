using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Donation.Domain.Dtos;
using Donation.Service.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static Donation.Common.Enums;

namespace DonationBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMembershipServices _membershipService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        

        public AccountController(IMembershipServices membershipService, IWebHostEnvironment hostingEnvironment)
        {
            _membershipService = membershipService;
            _hostingEnvironment = hostingEnvironment;          
        }        

        //Post: api/Account/Login
       [HttpPost]
       [Route("Login")]
        public IActionResult Get([FromBody] UserDto model)
        {            
            var result = _membershipService.Login(model.Email, model.Password);
            return Ok(result);
        }

        // POST: api/Account/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Post([FromBody] UserDto model)
        {
            var response = await _membershipService.RegisterUser(model);
            if (response.Status == (int)Number.One)
                return Ok(response);
            else
                return BadRequest(response);
        }

        // PUT: api/Account/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
