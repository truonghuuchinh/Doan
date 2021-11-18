using DoanApp.Commons;
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
using X.PagedList;

namespace DoanApp.Controllers
{
    [Authorize]
    public class PlayListVideoController : Controller
    {
        private readonly IPlayListService _playlistService;
        private readonly IDetailVideoService _detailService;
        private readonly IVideoService _videoService;
        public PlayListVideoController(IPlayListService playListService,IDetailVideoService detail,
            IVideoService videoService)
        {
            _playlistService = playListService;
            _detailService = detail;
            _videoService = videoService;
        }
        // GET: PlayListVideoController
        public ActionResult Index()
        {
            return View();
        }

      
        // POST: PlayListVideoController/Create
        [HttpPost]
        public async Task<ActionResult> Create(PlaylistRequest request)
        {
            if (request != null)
            {
                var result = await _playlistService.Create(request);
                if (result!=null)
                {
                        var playlist_vm = new DetailPlayListVideo();
                        playlist_vm.Id = result.Id;
                        playlist_vm.Name = result.Name;
                        playlist_vm.UserId = result.UserId;
                        playlist_vm.PosterVideo = null;
                        playlist_vm.Status = result.Status;
                        playlist_vm.CreateDate = result.CreateDate;
                        return Content(JsonConvert.SerializeObject(playlist_vm));
                }
            }
            return Content("null");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                var result = await _playlistService.Delete(id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateName(PlaylistRequest request)
        {
            if (request != null)
            {
                var result = await _playlistService.Update(request);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        
        // GET: PlayListVideoController/Edit/5
      public string ListJsonPlayList(int? page, string searchList=null)
        {
            int pageNumber = page ?? 1;
            int pageSize = 4;
            var list = _playlistService.GetAll();
          
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            var list_vm = _detailService.GetDetailPlayList(user, searchList);
           if(list_vm.Count>0)
            {
                return JsonConvert.SerializeObject(list_vm.ToPagedList(pageNumber, pageSize));
            }
            
            return null;  
        }
    }
}
