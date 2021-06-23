using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IPlayListService
    {
        List<PlayList> GetAll();
        Task<PlayList> Create(PlaylistRequest plRequest);
        Task<int> Update(PlaylistRequest plRequest);
        Task<int> Delete(int id);
        Task<PlayList> FindAsync(int id);
    }
}
