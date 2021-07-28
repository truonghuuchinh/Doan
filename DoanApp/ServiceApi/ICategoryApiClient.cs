using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.ServiceApi
{
    public interface ICategoryApiClient
    {
        Task<List<Category>> GetAll(string token,string nameSearch=null);
        Task<int> Create(string token, CategoryRequest request);
        Task<int> Update(string token, CategoryRequest request);
        Task<int> Delete(string token,int id);
    }
}
