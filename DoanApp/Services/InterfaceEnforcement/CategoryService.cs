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
    public class CategoryService : ICategoryService
    {
        private readonly DpContext _context;
        public CategoryService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> CreateAsync(CategoryRequest categoryRequest)
        {
            var category = new Category();
            category.Name = categoryRequest.Name;
            _context.Category.Add(category);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var category = _context.Category.FirstOrDefault(x => x.Id == id);
            if (category != null)
            {
                category.Status = category.Status ? false : true;
                _context.Update(category);
                return await _context.SaveChangesAsync();
            }
            return -1;
            
        }

        public async Task<Category> FinByIdAsync(int id)
        {
            return await _context.Category.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Category>> GetAll()
        {
            var list = await _context.Category.Where(x=>x.Status).ToListAsync();
            return list;
        }

        public async Task<List<Category>> GetAllApi()
        {
            var list = await _context.Category.ToListAsync();
            return list;
        }

        public async Task<int> UpdateAsync(CategoryRequest categoryRequest)
        {
            var category = _context.Category.FirstOrDefault(x => x.Id == categoryRequest.Id);
            if (category != null)
            {
                category.Name = categoryRequest.Name;
                category.Status = categoryRequest.Status;
                _context.Update(category);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

    }
}
