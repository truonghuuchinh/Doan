using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
   public interface ILikeCommentService
    {
        List<LikeCommentDetail> GetAll();
        Task<int> Create(LikeCommentRequest likeRequest);
        Task<int> Delete(int id);
        Task<LikeCommentDetail> FindLikeAsync(int userId,int videoId);
    }
}
