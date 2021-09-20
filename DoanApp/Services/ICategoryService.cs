using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllApi();
        Task<List<Category>> GetAll();
        Task<Category> FinByIdAsync(int id);
        Task<int> CreateAsync(CategoryRequest functionRequest);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(CategoryRequest functionRequest);
    }
}
