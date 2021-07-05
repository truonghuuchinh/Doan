using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface INotificationService
    {
        List<Notification> GetAll();
        Task<int> Create(NotificationRequest request,int toUserId);
        Task<int> Delete(int id);
        Task<Notification> FindAsync(int id);
        Task<int> UpdateWatched(int[] id,int userId);
        List<Notification> GetNotification(AppUser user);
        Task<int> CreateNotifiComment(Comment_vm video_user,Comment comment_fromUser,Video video);
        Task<int> UpdateStatus(int id);
        Task<int> CreateReplyReport(NotificationRequest request,int idAdmin);
    }
}
