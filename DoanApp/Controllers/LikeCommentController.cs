
using DoanApp.Models;
using DoanApp.Services;
using DoanData.Commons;
using DoanData.Models;
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
    public class LikeCommentController : Controller
    {
        private readonly ILikeCommentService _likeService;
        private readonly ICommentService _commentService;
        public LikeCommentController(ILikeCommentService likeService,ICommentService comment)
        {
            _likeService = likeService;
            _commentService = comment;
        }

        // POST: LikeCommentController/Create
        [HttpPost]
        public async Task<ActionResult> Create(string dataLike)
        {
            var like = JsonConvert.DeserializeObject<LikeCommentRequest>(dataLike);
            int result = 0;
            if (like.Reaction == Reactions.DontLike.ToString() || like.Reaction == Reactions.DontDisLike.ToString())
            {
                 var getLike = _likeService.FindLikeAsync(like.UserId, like.VideoId).Result;
                result = await _likeService.Delete(getLike.Id);
            }
            else
            {
                result = await _likeService.Create(like);
            }
            if (result > 0)
            {
                var resultUpdate = await _commentService.UpdateLike(like.IdComment,like.Reaction) ;
                if (resultUpdate > 0)
                {
                    int getLikes = 0;
                    var getVideo = await _commentService.Find(like.IdComment);
                    if (like.Reaction == "Like" || like.Reaction == Reactions.DontLike.ToString())
                        getLikes = getVideo.Like;
                    if (like.Reaction == "DisLike" || like.Reaction == Reactions.DontDisLike.ToString())
                        getLikes = getVideo.DisLike;
                    return Content(getLikes.ToString());
                }
            }
            return Content("Error");
        }

        // GET: LikeCommentController/Edit/5
       [HttpPost]
       public async Task<IActionResult> EditLike(string dataLike)
        {
            var like = JsonConvert.DeserializeObject<LikeCommentRequest>(dataLike);
            var results = await _commentService.UpdateLikeRevert(like.IdComment,like.Reaction);
            var searchLike =  _likeService.FindLikeAsync(like.UserId, like.VideoId).Result;
            var result = await _likeService.Delete(searchLike.Id);

            if (results > 0)
            {
                if (result > 0)
                {
                    int count = 0;
                    var getVideo = await _commentService.Find(like.IdComment);
                    if (like.Reaction == "Like")
                        count = getVideo.Like;
                    if (like.Reaction == "DisLike")
                        count = getVideo.DisLike;
                    return Content(count.ToString());
                }
            }
            return Content("Error");
        }
        public string ListJsonLikeComment()
        {
            return JsonConvert.SerializeObject(_likeService.GetAll());
        }
        // POST: LikeCommentController/Edit/5
    }
}
