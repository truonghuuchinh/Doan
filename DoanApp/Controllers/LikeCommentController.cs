
using DoanApp.Models;
using DoanApp.Services;
using DoanData.Commons;
using DoanData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        public LikeCommentController(ILikeCommentService likeService,ICommentService comment,
            UserManager<AppUser> userManager)
        {
            _likeService = likeService;
            _commentService = comment;
            _userManager = userManager;
        }

        // POST: LikeCommentController/Create
        [HttpPost]
        public async Task<ActionResult> Create(string dataLike)
        {
            var like = JsonConvert.DeserializeObject<LikeCommentRequest>(dataLike);
            int result = 0;
            var statusLike = "";
            if (like.Reaction == Reactions.DontLike.ToString() || like.Reaction == Reactions.DontDisLike.ToString())
            {
                if (like.Reaction == Reactions.DontLike.ToString()) statusLike = "Like";
                else statusLike = "DisLike";
                 var getLike =await _likeService.FindLikeAsync(like.IdComment, statusLike);
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
            var searchLike =await  _likeService.FindLikeAsync(like.IdComment,like.Reaction);
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
        public async Task<string> ListJsonLikeComment(int idVideo)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user!=null)
            {
                var list = _likeService.GetAll().Where(x=>x.UserId==user.Id&&x.VideoId==idVideo).ToList();
                return JsonConvert.SerializeObject(list);
            }
            return null;
        }
        // POST: LikeCommentController/Edit/5
    }
}
