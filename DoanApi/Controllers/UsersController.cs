using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService user)
        {
            _userService = user;
        }
        [HttpPost("authenticated")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticated([FromBody] LoginRequest requestLogin)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var request = new AppUserRequest
            {
                PasswordHash = requestLogin.PasswordHash,
                Email = requestLogin.Email

            };
            var result = await _userService.AuthenticatedApi(request);
            if (result == "-1")
                return BadRequest("Cannot find user in Database");
            if (result == "0") return BadRequest("Password or UserNam InCorrect");
            return Ok(result);
        }
        [HttpGet("{nameSearch}")]
        public async Task<IActionResult> GetAll(string nameSearch=null)
        {
            var listUser = _userService.GetAll();
            if (nameSearch != null)
            {
                nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower();
                listUser = listUser.Where(x => ConvertUnSigned.convertToUnSign(x.FirtsName + " " + x.LastName).ToLower().
                 Contains(nameSearch)).ToList();
            }
            return Ok(JsonConvert.SerializeObject(listUser));       
        }
        [HttpGet("UserAdmin")]
        public async Task<IActionResult> GetUserAdmin(string email,string nameSearch=null)
        {
            var listUser = _userService.GetUserAdmin(email);
            if (nameSearch != null)
            {
                nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower();
                listUser = listUser.Where(x => ConvertUnSigned.convertToUnSign(x.Name).ToLower().
                 Contains(nameSearch)).ToList();

            }
            return Ok(JsonConvert.SerializeObject(listUser));
        }
        [HttpGet("findUser")]
        public async Task<IActionResult> FindUser(string emailUser)
        {
            var user =  await _userService.FindUser(emailUser);
            if (user != null)
                return Ok(JsonConvert.SerializeObject(user));
            return BadRequest("Cannot find user In Database");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            if (result>0)
                return Ok();
            return BadRequest("a Error when delete user");
        }
    }
}
