using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class LikeVideoService : ILikeVideoService
    {
        private readonly DpContext _context;
        public LikeVideoService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(LikeVideoRequest likeRequest)
        {
            var like = new LikeVideoDetail();
            if (likeRequest != null)
            {
                like.Reaction = likeRequest.Reaction;
                like.UserId = likeRequest.UserId;
                like.VideoId = likeRequest.VideoId;
            }
            _context.LikeVideoDetail.Add(like);
            return await _context.SaveChangesAsync();
            

        }

        public async Task<int> Delete(int id)
        {
            var like = _context.LikeVideoDetail.FirstOrDefault(X => X.Id == id);
            _context.Remove(like);
            return await _context.SaveChangesAsync();
        }

        public async  Task<LikeVideoDetail> FindAsync(int userId,int videoId)
        {
            var likeVideo =  _context.LikeVideoDetail.FirstOrDefault(x => x.UserId==userId&&x.VideoId==videoId);
            return  likeVideo;
        }

        public LikeVideoDetail FindNguocAsync(int userId, int videoId, string reaction)
        {
            var likeVideo = _context.LikeVideoDetail.
                FirstOrDefault(x => x.UserId == userId && x.VideoId == videoId&&x.Reaction==reaction);
            return likeVideo;
        }

        public List<LikeVideoDetail> GeAll()
        {
            return _context.LikeVideoDetail.ToList();
        }

        public async Task<int> Update(LikeVideoRequest likeRequest)
        {
            var like = _context.LikeVideoDetail.FirstOrDefault(X => X.Id == likeRequest.Id);
            if (likeRequest != null)
            {
                like.Reaction = likeRequest.Reaction;
                like.UserId = likeRequest.UserId;
                like.VideoId = likeRequest.VideoId;
            }
            _context.Update(like);
            return await _context.SaveChangesAsync();
        }
    }
}
