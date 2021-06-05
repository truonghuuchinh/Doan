using DoanApp.Models;
using DoanApp.Services;
using DoanData.Commons;
using DoanData.DoanContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{
    public class LikeVideoController : Controller
    {

        private readonly ILikeVideoService _likeService;
        private readonly IVideoService _videoService; 
        public LikeVideoController(ILikeVideoService likeService,IVideoService video)
        {
            _likeService = likeService;
            _videoService = video;
        }

        // POST: LikeVideoController/Create
        [HttpPost]
        public async Task<IActionResult> Create(string likeDetail)
        {
            var like = JsonConvert.DeserializeObject<LikeVideoRequest>(likeDetail);
            int result = 0;
            if (like.Reaction == Reactions.DontLike.ToString() || like.Reaction == Reactions.DontDisLike.ToString())
            {
                var getLike = await _likeService.FindAsync(like.UserId, like.VideoId);
                result = await _likeService.Delete(getLike.Id);
            }
            else
            {
                result = await _likeService.Create(like);
            }

            if (result > 0)
            {
                var resultUpdate = await _videoService.UpdateLike(like.VideoId, like.Reaction);
                if (resultUpdate > 0)
                {
                    int getLike = 0;
                    var getVideo =await _videoService.FinVideoAsync(like.VideoId);
                    if (like.Reaction=="Like"||like.Reaction==Reactions.DontLike.ToString()) 
                        getLike=getVideo.Like;
                    if (like.Reaction == "DisLike" || like.Reaction == Reactions.DontDisLike.ToString())
                        getLike = getVideo.DisLike;
                    return Content(getLike.ToString());
                }  
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> getLikeNguocPhanUng(string data)
        {
            var like = JsonConvert.DeserializeObject<LikeVideoRequest>(data);
             var results= await _videoService.UpdateLikeReverse(like.VideoId, like.Reaction);
            var searchLike = _likeService.FindNguocAsync(like.UserId, like.VideoId, like.Reaction);
            var result =await  _likeService.Delete(searchLike.Id);
            
            if(result>0)
            {
              
                if (results > 0)
                {
                    int count = 0;
                    var getVideo = await _videoService.FinVideoAsync(like.VideoId);
                    if (like.Reaction == "Like")
                        count = getVideo.Like;
                    if (like.Reaction == "DisLike")
                        count = getVideo.DisLike;
                    return Content(count.ToString());
                }
                
            }
            return Content("Error");
        }
        // GET: LikeVideoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LikeVideoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LikeVideoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LikeVideoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
