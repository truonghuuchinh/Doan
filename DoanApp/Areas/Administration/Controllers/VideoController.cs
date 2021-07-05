using DoanApp.Commons;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;
        public VideoController(IVideoService video,IUserService user,ICommentService comment)
        {
            _videoService = video;
            _userService = user;
            _commentService = comment;
        }
        public IActionResult Index(int? id)
        {
            ViewBag.LinkVideo = id ?? 0;
            ViewBag.Active = 5;
            var listUser = _userService.GetAll();
            var listVideo = _videoService.GetAll();
            var listVideo_vm = _videoService.GetVideo_Vm(listVideo,listUser).OrderByDescending(x=>x.Id);
            ViewBag.ListCountComment = _commentService.GetCountCm().ToArray();
            return View(listVideo_vm.ToPagedList(1,8));
        }
        public IActionResult Index_Partial(int? page,string name=null)
        {
            int pageNumber = page ?? 1;
            var listUser = _userService.GetAll();
            var listVideo = _videoService.GetAll();
            var listVideo_vm = _videoService.GetVideo_Vm(listVideo, listUser).OrderByDescending(x => x.Id).ToList();
            if (name != null)
            {
                name = ConvertUnSigned.convertToUnSign(name).ToLower();
                listVideo_vm = listVideo_vm.Where(x => ConvertUnSigned.convertToUnSign(x.Name).
                ToLower().Contains(name)|| ConvertUnSigned.convertToUnSign(x.FirtsName+" "+ x.LastName).
                ToLower().Contains(name)).ToList();
            }
            ViewBag.ListCountComment = _commentService.GetCountCm().ToArray();
            return View(listVideo_vm.ToPagedList(pageNumber, 8));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                var result = await _videoService.Delete(id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            if (id != 0)
            {
                var result = await _videoService.UpdateStatus(id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        public async Task<IActionResult> DetailVideo(int id)
        {
            var video = await _videoService.FinVideoAsync(id);
            return View(video);
        }
    }
}
