
using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class LikeCommentService : ILikeCommentService
    {
        private readonly DpContext _context;
        public LikeCommentService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(LikeCommentRequest likeRequest)
        {
            var like = new LikeCommentDetail();
            if (likeRequest != null)
            {
                like.Comment = likeRequest.IdComment;
                like.Reaction = likeRequest.Reaction;
                like.UserId = likeRequest.UserId;
                like.VideoId = likeRequest.VideoId;
                _context.LikeComments.Add(like);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> Delete(int id)
        {
            var like = _context.LikeComments.FirstOrDefault(X => X.Id == id);
            _context.Remove(like);
            return await _context.SaveChangesAsync();
        }

        public  async Task<LikeCommentDetail> FindLikeAsync(int userId,int videoId)
        {
            var like = _context.LikeComments.FirstOrDefault(x=>x.UserId==userId&&x.VideoId==videoId);
            return like;
        }

        public List<LikeCommentDetail> GetAll()
        {
            return _context.LikeComments.ToList();
        }
    }
}
