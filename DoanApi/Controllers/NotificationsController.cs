using DoanApp.Commons;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DoanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notifiService;
        public NotificationsController(INotificationService notifiService)
        {
            _notifiService = notifiService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int? id=null)
        {
            var list = _notifiService.GetAll();
            if (id != null)
            {
                list = list.Where(x => x.UserId == (int)id).ToList();
            }
            return Ok(JsonConvert.SerializeObject(list));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var result = await _notifiService.Delete((int)id);
                if (result > 0) return Ok("Success");
            }
            return BadRequest("A error is occur when delete item in database");
        }
        [HttpPut("updateStatus")]
        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id != null)
            {
                var result = await _notifiService.UpdateStatus((int)id);
                if (result > 0)
                    return Ok(1);
            }
            return BadRequest("Update Not SuccessFully");
        }

    }
}
