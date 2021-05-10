using DoanApp.Models;
using DoanApp.Services;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly DpContext _context;
        public FunctionService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> CreateAsync(FunctionRequest functionRequest)
        {
            var funtion = new Function();
            funtion.Name = functionRequest.Name;
            _context.Function.Add(funtion);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var function =  _context.Function.FirstOrDefault(x=>x.Id==id);
            _context.Remove(function);
            return await _context.SaveChangesAsync();
        }

        public async Task<Function> FinByIdAsync(int id)
        {
            return await _context.Function.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Function>> GetAll()
        {
            var list= await _context.Function.ToListAsync();
            return list;
        }

        public async Task<int> UpdateAsync(FunctionRequest functionRequest)
        {
            var function = _context.Function.FirstOrDefault(x => x.Id == functionRequest.Id);
            function.Name = functionRequest.Name;
            _context.Update(function);
           return await _context.SaveChangesAsync();
        }
    }
}
