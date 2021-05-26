using DoanApp.Models;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IVideoService
    {
        List<Video> GetAll();
        Task<int> Create(VideoRequest videoRequest,List<IFormFile> listPost);
        Task<int> Update(VideoRequest videoRequest);
        Task<int> Delete(int id);
        Task<Video> FinVideoAsync(int id);
      
    }
}
