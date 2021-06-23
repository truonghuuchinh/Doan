using DoanApp.Commons;
using DoanApp.Models;
using DoanData.Commons;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class VideoService : IVideoService
    {
        private readonly DpContext _context;

        public VideoService(DpContext context)
        {
            _context = context;
        }
        public  async Task<Video> Create(VideoRequest videoRequest,List<IFormFile> listPost)
        {
            var video = new Video();
            if (videoRequest != null)
            {
                video.Name = videoRequest.Name;
                video.Description = videoRequest.Description;
                video.AppUserId = videoRequest.AppUserId;
                video.CreateDate = new GetDateNow().DateNow;
                video.HidenVideo = videoRequest.HidenVideo;
                video.CategorysId = videoRequest.CategorysId;
                _context.Video.Add(video);
                  _context.SaveChanges();
                var findVideo = _context.Video.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.Name.Contains(videoRequest.Name));
                if (listPost.Count > 0)
                {
                    var paths = "";
                    foreach (var item in listPost)
                    {
                        var filename = item.FileName.Split('.');
                        var name =  filename[filename.Length - 1].ToLower();
                        if (name.Contains("mp4"))
                        {
                            name = findVideo.Id.ToString() + "." + filename[filename.Length - 1].ToLower();
                            paths = "wwwroot/Client/video";
                            findVideo.LinkVideo = name;
                        }
                        else
                        {
                            name = findVideo.Id.ToString() + "." + filename[filename.Length - 1].ToLower();
                            findVideo.PosterImg = name;
                            paths = "wwwroot/Client/imgPoster";
                        }
                        using (var fileStream = new FileStream(Path.Combine(paths, name),
                         FileMode.Create, FileAccess.Write))
                        {

                            item.CopyTo(fileStream);
                        }
                    }
                    _context.Update(findVideo);
                    await _context.SaveChangesAsync();
                    return findVideo;
                }
            }
            return null;
        }

        public async Task<int> Delete(int id)
        {
            var video = await FinVideoAsync(id);
            video.Status = false;
            _context.Update(video);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteVideoFavorite(int id)
        {
            var Watched = _context.LikeVideoDetail.FirstOrDefault(X => X.Id == id);
            if (Watched != null)
            {
                _context.Remove(Watched);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<Video> FinVideoAsync(int id)
        {
            return await _context.Video.FirstOrDefaultAsync(x => x.Id == id&x.Status);
        }

        public List<Video> GetAll()
        {
            return  _context.Video.Where(x=>x.Status).ToList();
        }

        public List<Video_vm> GetAllVideoPlayList(List<Video> videos, AppUser user)
        {
            var listvm = new List<Video_vm>()
;            if (videos.Count > 0)
            {
                foreach (var item in videos)
                {
                    var video = new Video_vm();
                    video.PosterImg = item.PosterImg;
                    video.Name = item.Name;
                    video.Id = item.Id;
                    video.CategorysId = item.CategorysId;
                    video.LinkVideo = item.LinkVideo;
                    video.Avartar = user.Avartar;
                    video.HidenVideo = item.HidenVideo;
                    video.FirtsName = user.FirtsName;
                    video.Description = item.Description;
                    video.LastName = user.LastName;
                    video.AppUserId = user.Id;
                    video.Name = item.Name;
                    video.Status = item.Status;
                    video.ViewCount = item.ViewCount;
                    video.LoginExternal = user.LoginExternal;
                    video.CreateDate = item.CreateDate;
                    listvm.Add(video);
                }
                return listvm;
            }
            return null;
        }

        public List<Video_vm> GetVideo_Vm(List<Video> lVideo, List<AppUser> lUser)
        {
            List<Video_vm> listVideo_Vm = new List<Video_vm>();
            var listVideo = (from video in lVideo
                             join user in lUser on video.AppUserId equals user.Id
                             select new
                             {
                                 video,
                                 user
                             });

            foreach (var item in listVideo)
            {
                var video = new Video_vm();
                video.PosterImg = item.video.PosterImg;
                video.Name = item.video.Name;
                video.Id = item.video.Id;
                video.CategorysId = item.video.CategorysId;
                video.LinkVideo = item.video.LinkVideo;
                video.Avartar = item.user.Avartar;
                video.HidenVideo = item.video.HidenVideo;
                video.FirtsName = item.user.FirtsName;
                video.Description = item.video.Description;
                video.LastName = item.user.LastName;
                video.AppUserId = item.user.Id;
                video.Name = item.video.Name;
                video.Status = item.video.Status;
                video.ViewCount = item.video.ViewCount;
                video.LoginExternal = item.user.LoginExternal;
                video.CreateDate = item.video.CreateDate;
                listVideo_Vm.Add(video);
            }
            return listVideo_Vm;
        }

        public async Task<int> Update(VideoRequest videoRequest)
        {
            var video = await FinVideoAsync(videoRequest.Id);
           
            if (video != null)
            {
                videoRequest.Name = videoRequest.Name == "" ? video.Name : videoRequest.Name;
                videoRequest.Description = videoRequest.Description == "" ? video.Description : videoRequest.Description;
                videoRequest.PosterVideo = videoRequest.PosterVideo == "" ? video.PosterImg : videoRequest.PosterVideo;
                video.Name = videoRequest.Name;
                video.HidenVideo = videoRequest.HidenVideo;
                video.Description = videoRequest.Description;
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateCategory(int id,int idCategory)
        {
            var video = _context.Video.FirstOrDefault(X => X.Id == id);
            if (video != null&&idCategory!=0)
            {
                video.CategorysId = idCategory;
                _context.Update(video);
                return await _context.SaveChangesAsync();
            }
            return -1;
            
        }

        public async Task<int> UpdateLike(int idVideo,string reaction)
        {
            var video = _context.Video.FirstOrDefault(X => X.Id == idVideo);
            if (reaction == Reactions.Like.ToString()) video.Like += 1;
            if (reaction == Reactions.DisLike.ToString()) video.DisLike += 1;
            if (reaction == Reactions.DontLike.ToString()) video.Like-=1;
            if (reaction == Reactions.DontDisLike.ToString()) video.DisLike -= 1;
            if (video.Like < 0)
                video.Like = 0;
            if (video.DisLike < 0)
                video.DisLike = 0;
            _context.Update(video);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateLikeReverse(int idVideo, string reaction)
        {
            var video = await _context.Video.FindAsync(idVideo);
            if (reaction == "Like") video.Like -= 1;
            else video.DisLike -= 1;
            if (video.Like < 0)
                video.Like = 0;
            if (video.DisLike < 0)
                video.DisLike = 0;
            _context.Update(video);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdatePermission(VideoRequest request)
        {
            var video = _context.Video.FirstOrDefault(x => x.Id == request.Id);
            if (video != null)
            {
                if (request.HidenVideo) video.HidenVideo = true;
                else video.HidenVideo = false;
                _context.Update(video);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateView(int id)
        {
            var video = await _context.Video.FirstOrDefaultAsync(x => x.Id == id);
            video.ViewCount += 1;
            _context.Update(video);
            return await _context.SaveChangesAsync();
        }
        
    }
}
