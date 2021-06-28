using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
   public interface IDetailVideoService
    {
        List<DetailVideo> GetAll();
        Task<int> Create(DetailVideoRequest request);
        Task<int> Update(DetailVideoRequest request);
        Task<int> Delete(DetailVideoRequest request);
        Task<DetailVideo> FindAsync(int id);
        List<DetailPlayListVideo> GetDetailPlayList(AppUser user,string nameSearch);

    }
}
