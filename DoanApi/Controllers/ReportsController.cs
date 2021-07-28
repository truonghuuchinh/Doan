using DoanApp.Commons;
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
    public class ReportsController : ControllerBase
    {
        private readonly IReportVideoService _reportService;
        public ReportsController(IReportVideoService reportVideoService)
        {
            _reportService = reportVideoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string nameSearch=null)
        {
            var listReport_Vm = _reportService.GetList_Vm();
            if (nameSearch != null)
            {
                nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower();

                 listReport_Vm = listReport_Vm.Where(x => ConvertUnSigned.convertToUnSign(x.Content).
                  ToLower().Contains(nameSearch) || ConvertUnSigned.convertToUnSign(x.NamUser).
                  ToLower().Contains(nameSearch) || ConvertUnSigned.convertToUnSign(x.NameVideo).
                  ToLower().Contains(nameSearch)).ToList();
            }
            return Ok(JsonConvert.SerializeObject(listReport_Vm));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reportService.Delete(id);
            if (result > 0)
                return Ok();
            return BadRequest("A error occur when delete item! ");
        }
    }
}
