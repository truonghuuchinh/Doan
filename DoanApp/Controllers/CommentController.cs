using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService comment)
        {
            _commentService = comment;
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result =await _commentService.Delete(id);
            if (result!=null)
            {
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateContent(CommentRequest request)
        {
            if (request != null)
            {
                var result = await _commentService.UpdateContent(request);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
    }
}
