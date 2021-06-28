using DoanApp.Commons;
using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class DetailVideoService : IDetailVideoService
    {
        private readonly DpContext _context;
        public DetailVideoService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(DetailVideoRequest request)
        {
            var playlist = _context.PlayList.FirstOrDefault(X => X.Id == request.PlayListId);
            var detail = new DetailVideo();
            if (request != null&&playlist!=null)
            {
                detail.PlayListId = request.PlayListId;
                detail.VideoId = request.VideoId;
                _context.DetailVideo.Add(detail);
                return await _context.SaveChangesAsync();

            }
            return -1;
           
        }

        public async Task<int> Delete(DetailVideoRequest request)
        {
            var detail = _context.DetailVideo.FirstOrDefault(x => x.PlayListId==request.PlayListId&&x.VideoId==request.VideoId);
            if(detail!=null)
            {
                _context.Remove(detail);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
        public async Task<DetailVideo> FindAsync(int id)
        {
            return await _context.DetailVideo.FirstOrDefaultAsync(X => X.Id == id);
        }

        public List<DetailVideo> GetAll()
        {
            return _context.DetailVideo.ToList();
        }

        public List<DetailPlayListVideo> GetDetailPlayList(AppUser user,string nameSearch)
        {
            var list = _context.PlayList.ToList();
            var playlistNoVideo = _context.PlayList.Where(x => !_context.DetailVideo.Any(y => y.PlayListId == x.Id) && x.UserId == user.Id).ToList();
            if (nameSearch != null)
            {
                nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower().Trim();
                list = list.Where(x => ConvertUnSigned.convertToUnSign(x.Name).
                      ToLower().Contains(nameSearch)).ToList();
                playlistNoVideo = playlistNoVideo.Where(x => ConvertUnSigned.convertToUnSign(x.Name).
                     ToLower().Contains(nameSearch)).ToList();
            }
            var playlist = list.Where(x => x.UserId == user.Id).ToList();
            var detailPlayList = (from plist in playlist
                                  join detail in _context.DetailVideo on plist.Id equals detail.PlayListId
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
                                      join video in _context.Video on dlist.VideoId equals video.Id
                                      select new { dlist, video.PosterImg };
            var listComplete = (from countItem in listCountItem
                                join detail in detailPlayListVideo on countItem.Key equals detail.dlist.Id
                                join listUser in _context.AppUser.ToList() on detail.dlist.UserId equals listUser.Id
                                select new
                                {
                                    Count = countItem.Count,
                                    detail.dlist.Id,
                                    detail.dlist.Name,
                                    detail.dlist.Status,
                                    detail.dlist.UserId,
                                    detail.dlist.VideoId,
                                    detail.PosterImg,
                                    detail.dlist.CreateDate,
                                    listUser.FirtsName,
                                    listUser.LastName
                                }).ToList();
            var listDetail_vm = new List<DetailPlayListVideo>();

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
                i.FirtsName = item.FirtsName;
                i.LastName = item.LastName;
                if (listDetail_vm.Count > 0)
                {
                    if (!listDetail_vm.Any(x => x.Id == item.Id))
                        listDetail_vm.Add(i);
                }
                else listDetail_vm.Add(i);
            }
            var playlistNovideoComplete = (from novideo in playlistNoVideo
                                           join us in _context.AppUser on novideo.UserId equals us.Id
                                           select new
                                           {
                                               Count =0,
                                               novideo.Id,
                                               novideo.Name,
                                               novideo.Status,
                                               novideo.UserId,
                                               novideo.CreateDate,
                                               us.FirtsName,
                                               us.LastName
                                           }).ToList();
            var playlistNovideo_vm = new List<DetailPlayListVideo>();
            foreach (var item in playlistNovideoComplete)
            {
                var i = new DetailPlayListVideo();
                i.Id = item.Id;
                i.Name = item.Name;
                i.Status = item.Status;
                i.UserId = item.UserId;
                i.VideoId = 0;
                i.PosterVideo = null;
                i.CreateDate = item.CreateDate;
                i.CountItem = 0;
                i.FirtsName = item.FirtsName;
                i.LastName = item.LastName;
                playlistNovideo_vm.Add(i);
            }
            listDetail_vm.AddRange(playlistNovideo_vm);
            return listDetail_vm.OrderByDescending(x=>x.Id).ToList();
            
        }

        public async Task<int> Update(DetailVideoRequest request)
        {
            var detail = _context.DetailVideo.FirstOrDefault(x => x.Id == request.Id);
            if (detail != null)
            {
                detail.PlayListId = request.PlayListId;
                detail.VideoId = request.VideoId;
                _context.DetailVideo.Add(detail);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
    }
}
