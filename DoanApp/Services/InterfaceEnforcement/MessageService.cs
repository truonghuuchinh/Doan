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
    public class MessageService : IMessageService
    {
        private readonly DpContext _context;
        public MessageService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(MessageRequest request,bool admin)
        {
            if (request != null)
            {
                
                var message = new Message();
                message.ReceiverId = request.ReceiverId;
                message.SenderId = request.SenderId;
                message.Watched = request.Watched;
                message.Content = request.Content;
                message.CreateDate = new GetDateNow().DateNow;
                if (admin)
                {
                    message.UserName = "Admin";
                    message.LoginExternal = false; ;
                    message.Avartar = "avartarDefault.jpg";
                }
                else
                {
                    message.LoginExternal = request.LoginExternal;
                    message.Avartar = request.Avartar;
                    message.UserName = request.UserName;
                }

                _context.Message.Add(message);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> Delete(int id)
        {
            if (id != 0)
            {
                var message = _context.Message.FirstOrDefault(x => x.Id == id);
                if(message!=null)
                {
                    _context.Remove(message);
                    return await _context.SaveChangesAsync();
                }
            }
            return -1;
        }

        public async Task<Message> FindMessageAsync(int id)
        {
            var mesage = await _context.Message.FirstOrDefaultAsync(X => X.Id == id);
            if (mesage != null) return mesage;
            return null;
        }

        public List<Message> GetAll()
        {
            return _context.Message.ToList();
        }

        public List<Message> GetMessage_Partial(int senderId, int receiverId, bool flag = false)
        {
            var listMessage = GetAll().Where(x => x.SenderId == senderId && x.ReceiverId == receiverId ||
             x.SenderId == receiverId && x.ReceiverId == senderId).ToList();
            if (flag)
            {
                listMessage = listMessage.OrderByDescending(x => x.Id).Take(1).ToList();
            }
            return listMessage;
        }

        public List<Message_Vm> GetUserChat(int userId,List<AppUser> users)
        {
            var listGetUserChat = _context.Message.Where(x => x.SenderId == userId).GroupBy(x => x.ReceiverId).
                Select(x=>new { x.Key});
            var listUserChatRecei = _context.Message.Where(x => x.ReceiverId == userId).GroupBy(x => x.SenderId).
                Select(x => new { x.Key });
             var listOverView=listGetUserChat.Union(listUserChatRecei).ToList();
            var listMessage = new List<Message>();
            var listNoneMessage = new List<Message>();
            var listMessage_Vm = new List<Message_Vm>();
            var listAll = _context.Message.OrderByDescending(x => x.Id).ToList();
            var checkUserIntoSender = listAll.Any(X => X.SenderId == userId);
            if (checkUserIntoSender)
            {
                foreach (var j in listOverView)
                {
                    var flagExits = false;

                    foreach (var item in listAll)
                    {
                        var sender = item.ReceiverId;
                        var receiver = item.SenderId;
                        
                        if (j.Key == item.SenderId&&item.ReceiverId==userId||j.Key==item.ReceiverId&&item.SenderId==userId)
                        {
                            var ms = new Message();
                            if (item.SenderId != userId)
                            {
                                item.SenderId = sender;
                                item.ReceiverId = receiver;
                                
                                ms = item;
                               if(!item.Watched)
                                    ms.CheckWatched = true;
                            }
                            else
                                ms = GetUserInList(item);
                            flagExits = true;
                            if (listMessage.Count == 0)
                            {
                                listMessage.Add(ms);
                                break;
                            }
                            else
                            {

                                var checkItem = false;
                                if (item.SenderId != userId)
                                { 
                                    checkItem= listMessage.Any(x => x.ReceiverId == item.ReceiverId);
                                }else checkItem= listMessage.Any(x => x.SenderId == item.SenderId&&x.ReceiverId==item.ReceiverId);
                                if (!checkItem)
                                {
                                    listMessage.Add(ms);
                                    break;
                                }
                               
                            }
                        }

                    }

                    if (!flagExits)
                    {

                        foreach (var us in _context.AppUser.Where(x => x.Status))
                        {
                            var message = new Message();
                            if (j.Key == us.Id)
                            {
                                var checkUserAdmin = users.Any(X => X.Id == us.Id);
                                if (!checkUserAdmin)
                                {
                                    message.LoginExternal = false;
                                    message.Avartar ="avartarDefault.jpg";
                                    message.UserName = "Admin";
                                }
                                else
                                {
                                    message.LoginExternal = us.LoginExternal;
                                    message.Avartar = us.Avartar;
                                    message.UserName = us.FirtsName + " " + us.LastName;
                                }
                                message.SenderId = userId;
                                message.ReceiverId = j.Key;
                                foreach (var ms in listAll)
                                {
                                    if (j.Key == ms.ReceiverId)
                                    {
                                        message.Content = ms.Content;
                                        message.CreateDate = ms.CreateDate;
                                        break;
                                    }
                                }
                                if (listNoneMessage.Count == 0)
                                {
                                    listNoneMessage.Add(message);
                                }
                                else
                                {
                                    var checkItem2 = listNoneMessage.Contains(message);
                                    if (!checkItem2)
                                        listNoneMessage.Add(message);
                                }
                                break;
                            }
                           
                        }
                    }

                }
            }
            else
            {
                foreach (var ms in listAll)
                {
                    if (userId == ms.ReceiverId)
                    {
                        foreach (var us2 in _context.AppUser.Where(x => x.Status))
                        {
                                var message = new Message();
                            if (ms.SenderId == us2.Id)
                            {
                               
                                message.SenderId = userId;
                                message.ReceiverId = ms.SenderId;
                                message.Content = ms.Content;
                                message.CreateDate = ms.CreateDate;
                                if (ms.UserName == "Admin")
                                {
                                    message.Avartar = ms.Avartar;
                                    message.UserName = ms.UserName;
                                    message.LoginExternal = false;
                                }
                                else
                                {
                                    message.LoginExternal = us2.LoginExternal;
                                    message.Avartar = us2.Avartar;
                                    message.UserName = us2.FirtsName + " " + us2.LastName;
                                }
                                if(!ms.Watched) message.CheckWatched = true;
                                if (listNoneMessage.Count == 0)
                                    listNoneMessage.Add(message);
                                else
                                {
                                    var checknone = listNoneMessage.Any(X => X.ReceiverId == ms.SenderId);
                                    if(!checknone) listNoneMessage.Add(message);
                                }
                                break;
                            }
                            

                        }
                        
                    }

                }
                   
            }
            listMessage.AddRange(listNoneMessage);
            foreach (var item in listMessage)
            {
                var ms_vm = new Message_Vm();
                var checkUserAdmin = users.Any(X => X.Id == item.ReceiverId);
                if (checkUserAdmin)
                {
                    ms_vm.LoginExternal = false;
                    ms_vm.Avartar = "avartarDefault.jpg";
                    ms_vm.UserName = "Admin";
                }
                else
                {
                    ms_vm.LoginExternal = item.LoginExternal;
                    ms_vm.Avartar = item.Avartar;
                    ms_vm.UserName = item.UserName;
                }
                ms_vm.Content = item.Content;
                ms_vm.CreateDate = item.CreateDate;
                ms_vm.ReceiverId = item.ReceiverId;
                ms_vm.SenderId = item.SenderId;
                ms_vm.Watched = item.Watched;
                ms_vm.CheckWatched = item.CheckWatched;
                foreach (var us in UserAuthenticated.ListtUser)
                {
                    if (us.Id == item.ReceiverId)
                    {
                        ms_vm.Online = true;
                        break;
                    }
                       
                }
                listMessage_Vm.Add(ms_vm);
            }
           
            return listMessage_Vm.OrderByDescending(x=>x.Id).ToList();
        }
        public Message GetUserInList(Message message)
        {
            foreach (var us2 in _context.AppUser.Where(x=>x.Status))
            {
                if (us2.Id == message.ReceiverId)
                {
                    message.LoginExternal = us2.LoginExternal;
                    message.Avartar = us2.Avartar;
                    message.UserName = us2.FirtsName + " " + us2.LastName;
                    return message;
                }
            }
            return null;
        }

        public List<Message_Vm> GetUserNoneChat(List<Message_Vm> message_Vms,int userId,List<AppUser> users)
        {
            var listMessage_Vm = new List<Message_Vm>();
            var listUser = _context.AppUser.Where(x => x.Status && x.Id != userId).ToList();
            foreach (var item in users)
            {
                foreach (var item1 in listUser)
                {
                    if (item.UserName == item1.UserName)
                    {
                        listUser.Remove(item1);
                        break;
                    }
                }
            }
            foreach (var item in listUser)
            {
                var checkItem = message_Vms.Any(X => X.ReceiverId == item.Id);
                if (!checkItem)
                {
                    var ms_vm = new Message_Vm();
                    ms_vm.LoginExternal = item.LoginExternal;
                    ms_vm.Avartar = item.Avartar;
                    ms_vm.UserName = item.FirtsName + " " + item.LastName;
                    ms_vm.Content = "Trò chuyện ngay";
                    ms_vm.ReceiverId = item.Id;
                    ms_vm.SenderId = userId;
                    foreach (var us in UserAuthenticated.ListtUser)
                    {
                        if (item.Id == us.Id)
                        {
                            ms_vm.Online = true;
                            break;
                        }
                    }
                    listMessage_Vm.Add(ms_vm);
                }
            }
            return listMessage_Vm;
        }

        public async Task<int> Update(MessageRequest request)
        {
            var message = await _context.Message.FirstOrDefaultAsync(X => X.Id == request.Id);
            if (message != null)
            {
                message.Content = request.Content;
                _context.Update(message);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateWatched(int senderId, int receiverId,bool flag)
        {
            var countChange = 0;

            var listMessage = _context.Message.ToList();
            if (receiverId == 0)
            {
                listMessage = listMessage.Where(x => x.ReceiverId == senderId && !x.Watched).ToList();
            }
            else
            {
                if (!flag)
                    listMessage = listMessage.Where(x => x.ReceiverId == senderId && !x.Watched).ToList();
                else 
                   listMessage =listMessage.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId&&!x.Watched||
                     x.SenderId == receiverId && x.ReceiverId == senderId&&!x.Watched).ToList();
            }
            receiverId = receiverId == 0 ? senderId : receiverId;

            foreach (var item in listMessage)
            {
                if (item.ReceiverId == senderId)
                {
                    if (flag)
                    {
                        var message = _context.Message.FirstOrDefault(X => X.Id == item.Id);
                        message.Watched = true;
                        _context.Update(message);
                        await _context.SaveChangesAsync();
                    }
                    countChange++;
                }

            }
            return countChange;
  
        }
    }
}
