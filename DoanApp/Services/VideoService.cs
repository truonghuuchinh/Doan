using DoanApp.Models;
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
        public  async Task<int> Create(VideoRequest videoRequest,List<IFormFile> listPost)
        {
            var video = new Video();
            if (videoRequest != null)
            {
                video.Name = videoRequest.Name;
                video.Description = videoRequest.Description;
                video.AppUserId = videoRequest.AppUserId;
                video.CreateDate = DateTime.Now.ToString("MM-d-yyyy H:mm:ss");
                video.HidenVideo = videoRequest.HidenVideo;
                video.CategorysId = videoRequest.CategorysId;
                _context.Video.Add(video);
                  _context.SaveChanges();
                var findVideo = _context.Video.FirstOrDefault(x => x.Name.Contains(videoRequest.Name));
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
                    return await _context.SaveChangesAsync();
                }
            }
            return 0;
        }

        public async Task<int> Delete(int id)
        {
            var video = await FinVideoAsync(id);
            _context.Remove(video);
            return await _context.SaveChangesAsync();
        }

        public async Task<Video> FinVideoAsync(int id)
        {
            return await _context.Video.FirstOrDefaultAsync(x => x.Id == id);
        }

        public List<Video> GetAll()
        {
            return  _context.Video.ToList();
        }

      

        public async Task<int> Update(VideoRequest videoRequest)
        {
            var video = await FinVideoAsync(videoRequest.Id);
            if (videoRequest != null)
            {
                video.Name = videoRequest.Name;
                video.Description = videoRequest.Description;
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
