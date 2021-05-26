using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Controllers
{
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        public VideoController(IVideoService videoService, IUserService userService,
            ICategoryService category)
        {
            _videoService = videoService;
            _userService = userService;
            _categoryService = category;
        }
        // GET: VideoController
        public ActionResult Index()
        {

            return View();
        }
        public IActionResult MyChannel(int? page)
        {
            ViewData["Title"] = new SelectList(_categoryService.GetAll().Result, "Id", "Name");
            if (page == null) page = 1;
            var pageSize = 5;
            var pageNumber = page ?? 1;
            var user = _userService.FindUser(User.Identity.Name).Result;
            var list = _videoService.GetAll().
                Where(x=>x.AppUserId==user.Id).ToPagedList(pageNumber,pageSize);
            return View(list);
        }
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VideoRequest videoRequest,
            IFormFile PosterVideo,IFormFile LinkVideo,string HiddenVideo)
        {
            List<IFormFile> listPost = new List<IFormFile>();
            var user = await _userService.FindUser(User.Identity.Name);
            if (ModelState.IsValid)
            {
                if (PosterVideo != null && LinkVideo != null)
                {
                    listPost.Add(PosterVideo);
                    listPost.Add(LinkVideo);
                }
                if (user != null)
                {
                    videoRequest.AppUserId = user.Id;
                    videoRequest.HidenVideo = HiddenVideo.Contains("Public") ? true : false;
                    var result = await _videoService.Create(videoRequest,listPost);
                    if (result > 0)
                    {
                        return Redirect("MyChannel");
                    }
                }
              
            }
            return Redirect("MyChannel");
        }

        // GET: VideoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VideoController/Edit/5
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

        // GET: VideoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VideoController/Delete/5
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
