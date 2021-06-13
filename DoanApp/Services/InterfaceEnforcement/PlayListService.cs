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
    public class PlayListService : IPlayListService
    {
        private readonly DpContext _context;
        public PlayListService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(PlaylistRequest plRequest)
        {
            var playlist = new PlayList();
            if (plRequest != null)
            {
                playlist.Name = plRequest.Name;
                playlist.Status = plRequest.Status;
                playlist.UserId = plRequest.UserId;
                playlist.CreateDate = DateTime.Now.ToString("d-M-yyyy H:mm:ss");
                _context.PlayList.Add(playlist);
                return await _context.SaveChangesAsync();
            }
            return -1;  
        }

        public async Task<int> Delete(int id)
        {
            var playlist = _context.PlayList.FirstOrDefault(X => X.Id == id);
            _context.Remove(playlist);
            return await _context.SaveChangesAsync();
        }

        public async Task<PlayList> FindAsync(int id)
        {
            return await _context.PlayList.FirstOrDefaultAsync(x=>x.Id==id);
        }

        public List<PlayList> GetAll()
        {
            return _context.PlayList.ToList();
        }

        public async Task<int> Update(PlaylistRequest plRequest)
        {
            var playlist = _context.PlayList.FirstOrDefault(X => X.Id == plRequest.Id);
            if (playlist != null)
            {
                playlist.Name = plRequest.Name;
                playlist.Status = plRequest.Status;
                playlist.UserId = plRequest.UserId;
                playlist.CreateDate = plRequest.CreateDate;
                _context.PlayList.Add(playlist);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
    }
}
