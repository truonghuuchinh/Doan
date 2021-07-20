using DoanApp.Commons;
using DoanApp.Hubs;
using DoanApp.Models;
using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly UserManager<AppUser> _userManager;
        public MessageController(IUserService user,IMessageService message,UserManager<AppUser> usemanager)
        {
            _userService = user;
            _messageService = message;
            _userManager = usemanager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateWatched(int senderId,int receiverId,bool flag)
        {
            if (senderId != 0)
            {
                var result =await  _messageService.UpdateWatched(senderId, receiverId,flag);
                if (result > 0) return Content(result.ToString());
            }
            return Content("0");
        }
        public async Task<IActionResult> MessageList_Partial(int id)
        {
            var listUserAdmin = await _userManager.GetUsersInRoleAsync("Admin");

            var getuserChat = _messageService.GetUserChat(id,listUserAdmin.ToList());
            ViewBag.GetUserNoneChat = _messageService.GetUserNoneChat(getuserChat, id,listUserAdmin.ToList());
            return View(getuserChat);
        }
        [HttpPost]
        public async Task<IActionResult> Update(MessageRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _messageService.Update(request);
                if (result > 0) return Content("Success");
                
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                var result = await _messageService.Delete(id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        public string Getdate()
        {
            var date = CaculatorHours.Caculator(new GetDateNow().DateNow);
            return date;
        }
        public IActionResult Message_Partial(int senderId,int receiverId,bool flag=false)
        {    
            return View(_messageService.GetMessage_Partial(senderId,receiverId,flag));
        }
        public int GetNotifyMessage(int id)
        {
            var countList = _messageService.GetAll().Where(x => x.ReceiverId == id && !x.Watched).Count();
            return countList;
        }
        public async Task<IActionResult> Create(MessageRequest request,bool admin=false)
        {
            if (ModelState.IsValid)
            {
                var result = await _messageService.Create(request,admin);
                if (result > 0) return Ok();
            }
            return Content("Error");
        }

    }
}
