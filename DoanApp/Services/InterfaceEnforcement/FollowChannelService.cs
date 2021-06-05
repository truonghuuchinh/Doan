using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class FollowChannelService : IFollowChannelService
    {
        private readonly DpContext _contex;
        public FollowChannelService(DpContext context)
        {
            _contex = context;
        }
        public async Task<int> Create(FollowChannelRequest request)
        {
            var flChannel = new FollowChannel();
            flChannel.FromUserId = request.FromUserId;
            flChannel.ToUserId = request.ToUserId;
            _contex.FollowChannel.Add(flChannel);
            return await _contex.SaveChangesAsync();
        }

        public async Task<int> Delete(int fromUserId,int toUserId)
        {
            var flChannel = _contex.FollowChannel.FirstOrDefault(X => X.FromUserId==fromUserId&&X.ToUserId==toUserId);
            _contex.Remove(flChannel);
            return await _contex.SaveChangesAsync();
        }

        public List<FollowChannel> GetAll()
        {
            return _contex.FollowChannel.ToList();
        }
    }
}
