
using DoanApp.Commons;
using DoanApp.Models;
using DoanData.Commons;
using DoanData.DoanContext;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly DpContext _context;
        public CommentService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(CommentRequest cmRequest)
        {
            var comment = new Comment();
            if (cmRequest != null)
            {
                comment.Content=cmRequest.Content;
                comment.CreateDate = new GetDateNow().DateNow;
                comment.UserId = cmRequest.UserId;
                comment.ReplyForId = cmRequest.ReplyForId;
                comment.CommentId = cmRequest.CommentId;
                comment.VideoId = cmRequest.VideoId;
                comment.ReplyFor = cmRequest.ReplyFor;
                _context.Comment.Add(comment);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<List<int>> Delete(int id)
        {
            var listId = new List<int>();
            var comment = _context.Comment.FirstOrDefault(X => X.Id == id);
            if (comment != null)
            {
               
                foreach (var like in _context.LikeComments.ToList())
                {
                    if (like.Comment == comment.Id)
                        _context.Remove(like);
                }

                foreach (var item in GetAll())
                {
                    if (item.CommentId == comment.Id || item.ReplyForId == comment.Id)
                    {
                        listId.Add(item.Id);
                        _context.Remove(item);
                    }
                }
                listId.Add(comment.Id);
                _context.Remove(comment);
                 await _context.SaveChangesAsync();
                return listId;
            }
            return null;
        }

        public async Task<Comment> Find(int id)
        {
            var comment= await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                return comment;
            }
            return null;
        }

        public List<Comment> GetAll()
        {
            var list = _context.Comment.ToList();
            return list;
        }

        public List<Comment_vm> GetAll_vm(List<AppUser> user, List<Comment> comments)
        {
            List<Comment_vm> listCm_vm = new List<Comment_vm>();
            
            var list = from cm in comments
                       join us in user on cm.UserId equals us.Id
                       select new
                       {
                           cm,
                           us
                       };
            foreach (var item in list)
            {
                var cm_vm = new Comment_vm();
                cm_vm.Id = item.cm.Id;
                cm_vm.Content = item.cm.Content;
                cm_vm.CreateDate = item.cm.CreateDate;
                cm_vm.UserId = item.cm.UserId;
                cm_vm.CommentId = item.cm.CommentId;
                cm_vm.VideoId = item.cm.VideoId;
                cm_vm.Like = item.cm.Like;
                cm_vm.DisLike = item.cm.DisLike;
                cm_vm.Avartar = item.us.Avartar;
                cm_vm.FirtsName = item.us.FirtsName;
                cm_vm.LastName = item.us.LastName;
                cm_vm.ReplyFor = item.cm.ReplyFor;
                cm_vm.LoginExternal = item.us.LoginExternal;
                listCm_vm.Add(cm_vm);
            }
            return listCm_vm;
        }

        public  Comment_vm GetCm_Vm(CommentRequest cmRequest)
        {
            var cm = _context.Comment.OrderByDescending(x => x.Id).
                FirstOrDefault(x => x.Content.Contains(cmRequest.Content) 
                && x.UserId == cmRequest.UserId);
            var us = _context.AppUser.FirstOrDefault(x=>x.Id==cm.UserId);
            var cm_vm = new Comment_vm
            {
                Id = cm.Id,
                Content = cm.Content,
                CreateDate = cm.CreateDate,
                UserId = cm.UserId,
                CommentId = cm.CommentId,
                VideoId = cm.VideoId,
                ReplyFor=cm.ReplyFor,
                Like = cm.Like,
                DisLike = cm.DisLike,
                Avartar =us.Avartar,
                FirtsName = us.FirtsName,
                LastName = us.LastName,
                LoginExternal = us.LoginExternal
               };

            return  cm_vm;
        }

        public List<CountComment> GetCountCm()
        {
            var listCount = new List<CountComment>();
            var listCountComment = (from vd in _context.Video.ToList()
                                    join cm in GetAll() on vd.Id equals cm.VideoId
                                    group cm by cm.VideoId into grp
                                    select new
                                    {
                                        grp.Key,
                                        Count = grp.Count()
                                    }).OrderByDescending(x => x.Key).ToList();
            foreach (var item in listCountComment)
            {
                var i = new CountComment();
                i.Id = item.Key;
                i.Count = item.Count;
                listCount.Add(i);
            }
            return listCount;
        }

        public async Task<int> Update(CommentRequest cmRequest)
        {
            var comment = _context.Comment.FirstOrDefault(X => X.Id == cmRequest.Id);
            if (cmRequest != null)
            {
                comment.Content = cmRequest.Content;
                comment.CreateDate = DateTime.Now.ToString("MM-d-yyyy H:mm:s");
                comment.UserId = cmRequest.UserId;
                comment.CommentId = cmRequest.CommentId;
                comment.VideoId = cmRequest.VideoId;
                comment.Like = cmRequest.Like;
                comment.DisLike = cmRequest.DisLike;
                
                _context.Update(comment);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> UpdateContent(CommentRequest cmRequest)
        {
            var cm = _context.Comment.FirstOrDefault(X => X.Id == cmRequest.Id);
            if (cm != null)
            {
                cm.Content = cmRequest.Content;
                _context.Update(cm);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateLike(int id,string reaction)
        {
            var comment = _context.Comment.FirstOrDefault(x=>x.Id==id);
            if (reaction == Reactions.Like.ToString()) comment.Like += 1;
            if (reaction == Reactions.DisLike.ToString()) comment.DisLike += 1;
            if (reaction == Reactions.DontLike.ToString()) comment.Like -= 1;
            if (reaction == Reactions.DontDisLike.ToString()) comment.DisLike -= 1;
            if (comment.Like < 0)
                comment.Like = 0;
            if (comment.DisLike < 0)
                comment.DisLike = 0;
            _context.Update(comment);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateLikeRevert(int id,string reaction)
        {
            var comment = _context.Comment.FirstOrDefault(x => x.Id==id);
            if (reaction == Reactions.Like.ToString()) comment.Like -= 1;
            if (reaction == Reactions.DisLike.ToString()) comment.DisLike -= 1;
            if (comment.Like < 0)
                comment.Like = 0;
            if (comment.DisLike < 0)
                comment.DisLike = 0;
            _context.Update(comment);
            return await _context.SaveChangesAsync();
        }
    }
}
