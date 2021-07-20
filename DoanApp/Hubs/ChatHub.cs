using DoanApp.Models;
using DoanData.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.Users(message.SenderId.ToString(), message.ReceiverId.ToString()).SendAsync("receiveMessage", message);
        }
      
    }
}
