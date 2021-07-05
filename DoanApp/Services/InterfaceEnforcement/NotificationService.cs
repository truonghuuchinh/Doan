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
    public class NotificationService : INotificationService
    {
        private readonly DpContext _context;
        public NotificationService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(NotificationRequest request,int toUserId)
        {
            var listFollow = _context.FollowChannel.Where(x => x.ToUserId == toUserId).ToList();
            if (request != null&&listFollow.Count>0)
            {
                foreach (var item in listFollow)
                {
                    if (item.Notifications)
                    {
                        var notifi = new Notification();
                        notifi.AvartarUser = request.AvartarUser;
                        notifi.UserId = request.UserId;
                        notifi.FromUserId = item.FromUserId;
                        notifi.LoginExternal = request.LoginExternal;
                        notifi.PoterImg = request.PoterImg;
                        notifi.Content = "đã tải lên:" + request.Content;
                        notifi.VideoId = request.VideoId;
                        notifi.Watched = true;
                        notifi.CreateDate = DateTime.Now.ToString("MM-d-yyyy H:mm:ss");
                        notifi.Status = request.Status;
                        notifi.UserName = request.UserName;
                        _context.Notification.Add(notifi);
                    }
                }
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> CreateNotifiComment(Comment_vm video_user, Comment comment_fromUser, Video video)
        {
            if(video_user!=null&&comment_fromUser!=null&&video!=null)
            {
                var user = _context.AppUser.FirstOrDefault(X => X.Id == video_user.UserId);
                if (user != null)
                {
                    var notifi = new Notification();
                    notifi.AvartarUser = user.Avartar;
                    notifi.UserId = video_user.UserId;
                    notifi.FromUserId = comment_fromUser.UserId;
                    notifi.LoginExternal = user.LoginExternal;
                    notifi.PoterImg = video.PosterImg;
                    notifi.Content = ": Đã trả lời bình luận "+ comment_fromUser.Content+" của bạn";
                    notifi.VideoId = video.Id;
                    notifi.Status = true;
                    notifi.CreateDate = new GetDateNow().DateNow;
                    notifi.UserName = user.FirtsName + " " + user.LastName;
                    _context.Notification.Add(notifi);
                    return await _context.SaveChangesAsync();
                }
            }
            return -1;
        }

        public async Task<int> CreateReplyReport(NotificationRequest request,int idAmdin)
        {
            if (request != null)
            {
                var notifi = new Notification();
                notifi.AvartarUser = "avartarDefault.jpg";
                notifi.UserId = idAmdin;
                notifi.FromUserId = request.FromUserId;
                notifi.LoginExternal = false;
                notifi.PoterImg = request.PoterImg;
                notifi.Content = request.Content;
                notifi.VideoId = request.VideoId;
                notifi.Watched = true;
                notifi.CreateDate = DateTime.Now.ToString("MM-d-yyyy H:mm:ss");
                notifi.Status = request.Status;
                notifi.UserName = "Admin";
                _context.Notification.Add(notifi);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> Delete(int id)
        {
            var notifi = _context.Notification.FirstOrDefault(X => X.Id == id);
            if (notifi != null)
            {
                _context.Remove(notifi);
                return await _context.SaveChangesAsync(); 
            }
            return -1;
        }

        public async Task<Notification> FindAsync(int id)
        {
            return await _context.Notification.FirstOrDefaultAsync(X => X.Id == id);
        }

        public List<Notification> GetAll()
        {
            return _context.Notification.ToList();
        }

        

        public List<Notification> GetNotification(AppUser user)
        {
            if (user != null)
            {
                var listNotification = GetAll().Where(x => x.Status && x.FromUserId == user.Id).OrderByDescending(x => x.Id).ToList();
                return listNotification;
            }
            return null;
        }

        public async Task<int> UpdateStatus(int id)
        {
            var notifi = _context.Notification.FirstOrDefault(X => X.Id == id);
            if (notifi != null)
            {
                notifi.Status = false;
                _context.Update(notifi);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateWatched(int[] id, int userId)
        {
            
            for (int i = 0; i < id.Length; i++)
            {
                foreach (var item in GetAll().Where(x=>x.FromUserId==userId).ToList())
                {
                    if (item.Id == id[i]&&item.Watched)
                    {
                        item.Watched = false;
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return 1;
        }
    }
}
