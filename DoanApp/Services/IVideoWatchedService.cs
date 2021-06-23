using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IVideoWatchedService
    {
        List<VideoWatched> GetAll();
        Task<int> Create(VideoWatchedRequest request);
        Task<int> Delete(int id);
        Task<int> DeleteFavorite(int id);
        Task<int> GetJsonList();
        Task<VideoWatched> FindAsync(VideoWatchedRequest request);
        Task<LikeVideoDetail> FindFavorite(LikeVideoRequest request);
    }
}
