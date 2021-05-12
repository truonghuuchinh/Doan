using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class ActionService:IActionService
    {
        private readonly DpContext _context;
        public ActionService(DpContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(ActionRequest actionRequest)
        {
            
            var action = new DoanData.Models.Action();
            action.Name = actionRequest.Name;
            action.FunctionsId = actionRequest.FunctionsId;
            _context.Actions.Add(action);
           return await _context.SaveChangesAsync();
        }

        public async  Task<int> DeleteAsync(int id)
        {
            var function = _context.Actions.FirstOrDefault(x => x.Id == id);
            _context.Remove(function);
            return await _context.SaveChangesAsync();
        }

        public async Task<DoanData.Models.Action> FinByIdAsync(int id)
        {
            return await _context.Actions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public List<DoanData.Models.Action> GetAll()
        {
            return  _context.Actions.Include(x=>x.function).ToList();
        }


        public async Task<int> UpdateAsync(ActionRequest actionRequest)
        {
            var action = _context.Actions.FirstOrDefault(x => x.Id == actionRequest.Id);
            action.Name = actionRequest.Name;
            action.FunctionsId = actionRequest.FunctionsId;
            _context.Update(action);
            return await _context.SaveChangesAsync();
        }

    }
}
