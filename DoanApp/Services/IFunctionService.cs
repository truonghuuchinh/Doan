using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IFunctionService
    {
        Task<List<Function>> GetAll();
        Task<Function> FinByIdAsync(int id);
        Task<int> CreateAsync(FunctionRequest functionRequest);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(FunctionRequest functionRequest);


    }
}
