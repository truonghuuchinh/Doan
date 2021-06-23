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
        public async Task<ActionResult> Create(PlaylistRequest request,bool normal=false)
        {
            if (request != null)
            {
                var result = await _playlistService.Create(request);
                if (result!=null)
                {
                    if(normal) return RedirectToAction("Index", "Home");
                    else
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
                
            }
            return RedirectToAction("Index", "Home");
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
            if (searchList != null)
            {
                searchList = ConvertUnSigned.convertToUnSign(searchList).ToLower().Trim();
                list = list.Where(x => ConvertUnSigned.convertToUnSign(x.Name).
                      ToLower().Contains(searchList)).ToList();
            }
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            var playlist = list.Where(x => x.UserId == user.Id).
                OrderByDescending(x => x.Id).ToList();
            var detailPlayList = (from plist in playlist
                                  join detail in _detailService.GetAll() on plist.Id equals detail.PlayListId
                                  select new
                                  {
                                      plist.Id,
                                      plist.UserId,
                                      plist.Name,
                                      plist.Status,
                                      detail.VideoId,
                                      plist.CreateDate
                                  }).ToList();
            var listCountItem = from detail in detailPlayList
                                group detail by detail.Id into grp
                                select new
                                {
                                    Key = grp.Key,
                                    Count = grp.Count()
                                };
            var detailPlayListVideo = from dlist in detailPlayList
                                      join video in _videoService.GetAll() on dlist.VideoId equals video.Id
                                      select new { dlist, video.PosterImg };
            var listComplete = (from countItem in listCountItem
                                join detail in detailPlayListVideo on countItem.Key equals detail.dlist.Id
                                select new
                                {
                                    Count = countItem.Count,
                                    detail.dlist.Id,
                                    detail.dlist.Name,
                                    detail.dlist.Status,
                                    detail.dlist.UserId,
                                    detail.dlist.VideoId,
                                    detail.PosterImg
                                    ,
                                    detail.dlist.CreateDate
                                }).ToList();
            var listDetail_vm = new List<DetailPlayListVideo>();
            if (listComplete.Count > 0)
            {
                foreach (var item in listComplete)
                {
                    var i = new DetailPlayListVideo();
                    i.Id = item.Id;
                    i.VideoId = item.VideoId;
                    i.Status = item.Status;
                    i.UserId = item.UserId;
                    i.Name = item.Name;
                    i.PosterVideo = item.PosterImg;
                    i.CountItem = item.Count;
                    i.CreateDate = item.CreateDate;
                    if (listDetail_vm.Count > 0)
                    {
                        if (!listDetail_vm.Any(x => x.Id == item.Id))
                            listDetail_vm.Add(i);
                    }
                    else listDetail_vm.Add(i);
                }
              
               
            }
            var playlistNoVideo = _playlistService.GetAll().Where(x => !_detailService.GetAll().Any(y => y.PlayListId == x.Id) && x.UserId == user.Id).ToList();
            if (searchList != null)
            {
                playlistNoVideo = playlistNoVideo.Where(x => ConvertUnSigned.convertToUnSign(x.Name).ToLower().Contains(searchList)).ToList();
            }
            var playlistNovideo_vm = new List<DetailPlayListVideo>();
            foreach (var item in playlistNoVideo)
            {
                var i = new DetailPlayListVideo();
                i.Id = item.Id;
                i.VideoId = 0;
                i.Status = item.Status;
                i.UserId = item.UserId;
                i.Name = item.Name;
                i.PosterVideo = null;
                i.CountItem = 0;
                i.CreateDate = item.CreateDate;
                playlistNovideo_vm.Add(i);
            }
           if(listDetail_vm.Count>0|| playlistNovideo_vm.Count>0)
            {
                 listDetail_vm.AddRange(playlistNovideo_vm);
                listDetail_vm = listDetail_vm.OrderByDescending(x => x.Id).ToList();
                return JsonConvert.SerializeObject(listDetail_vm.ToPagedList(pageNumber, pageSize));
            }
            
            return null;  
        }
    }
}
