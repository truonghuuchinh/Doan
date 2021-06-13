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
    public class DetailVideoService : IDetailVideoService
    {
        private readonly DpContext _context;
        public DetailVideoService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(DetailVideoRequest request)
        {
            var detail = new DetailVideo();
            if (request != null)
            {
                detail.PlayListId = request.PlayListId;
                detail.VideoId = request.VideoId;
                _context.DetailVideo.Add(detail);
                return await _context.SaveChangesAsync();

            }
            return -1;
           
        }

        public async Task<int> Delete(DetailVideoRequest request)
        {
            var detail = _context.DetailVideo.FirstOrDefault(x => x.PlayListId==request.PlayListId&&x.VideoId==request.VideoId);
            if(detail!=null)
            {
                _context.Remove(detail);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
        public async Task<DetailVideo> FindAsync(int id)
        {
            return await _context.DetailVideo.FirstOrDefaultAsync(X => X.Id == id);
        }

        public List<DetailVideo> GetAll()
        {
            return _context.DetailVideo.ToList();
        }

        public async Task<int> Update(DetailVideoRequest request)
        {
            var detail = _context.DetailVideo.FirstOrDefault(x => x.Id == request.Id);
            if (detail != null)
            {
                detail.PlayListId = request.PlayListId;
                detail.VideoId = request.VideoId;
                _context.DetailVideo.Add(detail);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
    }
}
