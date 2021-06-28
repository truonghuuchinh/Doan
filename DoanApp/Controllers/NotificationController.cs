using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notifiService;
        private readonly IUserService _userService;
        private readonly IFollowChannelService _followChannel;
        public NotificationController(INotificationService notification,
            IUserService userService,IFollowChannelService follow)
        {
            _notifiService = notification;
            _userService = userService;
            _followChannel = follow;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateWatched(int[] IdNotifi)
        {
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            var result = await _notifiService.UpdateWatched(IdNotifi, user.Id);
            if (result > 0) return Content("Success");
            return Content("Error");
        }
        public async Task<IActionResult> Notification_Partial()
        {
            var flag = false;
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            var listNotifi = _notifiService.GetAll().Where(x => x.FromUserId == user.Id&&x.Status).OrderByDescending(x => x.Id).ToList();
            foreach (var item in listNotifi)
            {
                if (item.Watched)
                {
                    flag = true;
                    break;
                }
            }
            if (flag) return View(listNotifi);
            return Content("noNotifi");

        }
        public string GetCountNotifi()
        {
            var userss = UserAuthenticated.GetUser(User.Identity.Name);
            if (User != null)
            {
                var countnoti = _notifiService.GetNotification(userss).Where(x => x.Watched).Count();
                return countnoti.ToString();
            }
            return null;
        }
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result =await _notifiService.UpdateStatus(id);
            if (result > 0) return Content("Success");
            return Content("Error");
        }
        public async Task<IActionResult> UpdateFollow(FollowChannelRequest follow)
        {
            var result = await _followChannel.UpdateNotifi(follow);
            if (result > 0) return Content("Success");
            return Content("Error");
        }
    }
}
