using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface ILikeVideoService
    {
        List<LikeVideoDetail> GeAll();
        Task<int> Create(LikeVideoRequest likeRequest);
        Task<int> Update(LikeVideoRequest likeRequest);
        Task<int> Delete(int id);
        Task<LikeVideoDetail> FindAsync(int userId,int videoId);
        LikeVideoDetail FindNguocAsync(int userId, int videoId,string reaction);
    }
}
