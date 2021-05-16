using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoanApp.Models;
using DoanData.Models;

namespace DoanApp.Services
{
    public interface IActionService
    {
        List<DoanData.Models.Action> GetAll();
     
        Task<DoanData.Models.Action> FinByIdAsync(int id);
        Task<int> CreateAsync(ActionRequest functionRequest);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(ActionRequest functionRequest);
    }
}
