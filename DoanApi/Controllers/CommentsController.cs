using DoanApp.Models;
using DoanApp.Services;
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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int? id)
        {
            var listComment = _commentService.GetAll();
            if (id != null)
            {
                listComment = listComment.Where(x => x.UserId == (int)id).ToList();
               
            }
            return Ok(JsonConvert.SerializeObject(listComment));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id != null)
            {
                var result =await _commentService.Delete((int)id);
                if (result.Count > 0)
                    return Ok(1);
            }
            return BadRequest("A error is occur when delete Item");
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CommentRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _commentService.Create(request);
                if (result > 0) return Ok(1);
            }
            return BadRequest("Create No SuccessFully");
        }

    }
}
