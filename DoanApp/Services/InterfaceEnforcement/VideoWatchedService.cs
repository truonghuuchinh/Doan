using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class VideoWatchedService : IVideoWatchedService
    {
        private readonly DpContext _context;
        public VideoWatchedService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(VideoWatchedRequest request)
        {
            var watched = new VideoWatched();
            if (request != null)
            {
                var checkWatched = _context.VideoWatched.
                    FirstOrDefault(X => X.VideoId == request.VideoId && X.UserId == request.UserId);
                if (checkWatched == null)
                {
                    watched.UserId = request.UserId;
                    watched.VideoId = request.VideoId;
                    _context.VideoWatched.Add(watched);
                    return await _context.SaveChangesAsync();
                }
            }
            return -1;
        }

        public async Task<int> Delete(int id)
        {
            var Watched = _context.VideoWatched.FirstOrDefault(X => X.Id == id);
            if (Watched != null)
            {
                _context.Remove(Watched);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
        public async Task<int> DeleteFavorite(int id)
        {
            var Like = _context.LikeVideoDetail.FirstOrDefault(X => X.Id == id);
            if (Like != null)
            {
                _context.Remove(Like);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<VideoWatched> FindAsync(VideoWatchedRequest request)
        {
            var watched = _context.VideoWatched.FirstOrDefault(X => X.VideoId == request.VideoId && X.UserId == request.UserId);
            return watched;
        }

        public async Task<LikeVideoDetail> FindFavorite(LikeVideoRequest request)
        {
            var like= _context.LikeVideoDetail.FirstOrDefault(x => x.UserId == request.UserId &&
            x.VideoId == request.VideoId && x.Reaction == "Like");
            return like;
        }

        public List<VideoWatched> GetAll()
        {
            return _context.VideoWatched.ToList();
        }

        public Task<int> GetJsonList()
        {
            throw new NotImplementedException();
        }
    }
}
