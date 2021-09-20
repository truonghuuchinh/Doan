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

namespace DoanApp.Controllers
{
    [Authorize]
    public class ReportVideoController : Controller
    {
        private readonly IReportVideoService _reportVideo;
        public ReportVideoController(IReportVideoService reportVideo)
        {
            _reportVideo = reportVideo;
        }
        // POST: ReportVideoController/Create
        [HttpPost]
        public async Task<ActionResult> Create(string data)
        {
            var request = JsonConvert.DeserializeObject<ReportVideoRequest>(data);
            if (request != null)
            {
                var result =await  _reportVideo.Create(request);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }

      
    }
}
