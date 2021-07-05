using DoanApp.Areas.Administration.Models;
using DoanApp.Commons;
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
                report.CreateDate = new GetDateNow().DateNow;
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

        public List<ReportVideo_Vm> GetList_Vm()
        {
            var listReport_Vm = new List<ReportVideo_Vm>();
            var listReport = (from rp in _context.ReportVideo
                              join vd in _context.Video on rp.VideoId equals vd.Id
                              join us in _context.AppUser.Where(x=>x.Status) on rp.UserId equals us.Id
                              select new
                              {
                                  rp.Id,
                                  rp.Content,
                                  NameVideo = vd.Name,
                                  vd.PosterImg,
                                  VideoId = vd.Id,
                                  UserId = us.Id,
                                  UserName = us.FirtsName + " " + us.LastName,
                                  rp.CreateDate

                              });
            foreach (var item in listReport)
            {
                var report = new ReportVideo_Vm();
                report.Id = item.Id;
                report.Content = item.Content;
                report.NameVideo = item.NameVideo;
                report.ImgPoster = item.PosterImg;
                report.VideoId = item.VideoId;
                report.UserId = item.UserId;
                report.NamUser = item.UserName;
                report.CreateDate = item.CreateDate;
                listReport_Vm.Add(report);

            }
            return listReport_Vm.OrderByDescending(x=>x.Id).ToList();
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
