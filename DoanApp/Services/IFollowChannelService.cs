using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IFollowChannelService
    {
        List<FollowChannel> GetAll();
        Task<int> Create(FollowChannelRequest request);
        Task<int> Delete(int fromUser,int toUserId);
        Task<int> UpdateNotifi(FollowChannelRequest request);
    }
}
