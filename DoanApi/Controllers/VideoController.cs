using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        public VideoController(IVideoService video)
        {
            _videoService = video;
        }
        //localhost:port/video
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_videoService.GetAll());
        }
        //localhost:port/video/findvideo
        [HttpGet("findvideo/{id}")]
        public async Task<IActionResult> GetById([FromQuery]int? id)
        {
            var video = await _videoService.FinVideoAsync((int)id);
            return Ok(video);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] VideoRequest request, List<IFormFile> file)
        {
            var result = await _videoService.Create(request, file);
            if (result != null) return Ok(result);
            return BadRequest("Create No Success");
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VideoRequest request)
        {
            var result = await _videoService.Update(request);
            if (result>0) return Ok();
            return BadRequest("Update No Success");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _videoService.Delete(id);
            if (result > 0) return Ok();
            return BadRequest("Delete No Success");
        }
    }
}
