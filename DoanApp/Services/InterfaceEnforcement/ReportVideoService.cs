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
    public class ReportVideoService : IReportVideoService
    {
        private readonly DpContext _context;
        public ReportVideoService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(ReportVideoRequest request)
        {
            var report = new ReportVideo();
            if (request != null)
            {
                report.Content = request.Content;
                report.UserId = request.UserId;
                report.VideoId = request.VideoId;
                report.CreateDate = DateTime.Now.ToString("d-MM-yyyy H:mm:ss");
                _context.ReportVideo.Add(report);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> Delete(int id)
        {
            var report = _context.ReportVideo.FirstOrDefault(X => X.Id == id);
            if (report != null)
            {
                _context.Remove(report);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<ReportVideo> FinByIdAsync(int id)
        {
            return await _context.ReportVideo.FirstOrDefaultAsync(X => X.Id == id);
        }

        public List<ReportVideo> GetAll()
        {
            return _context.ReportVideo.ToList();
        }

        public async Task<int> Update(ReportVideoRequest request)
        {
            var report = _context.ReportVideo.FirstOrDefault(X => X.Id == request.Id);
            if (report != null)
            {
                report.Content = request.Content;
                report.UserId = request.UserId;
                report.VideoId = request.VideoId;
                _context.ReportVideo.Add(report);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
    }
}
