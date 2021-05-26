using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface ICommentService
    {
        List<Comment> GetAll();
        Task<int> Create(CommentRequest cmRequest);
        Task<int> Update(CommentRequest cmRequest);
        Task<int> Delete(int id);
        Comment_vm GetCm_Vm(CommentRequest cmRequest);
        List<Comment_vm> GetAll_vm(List<AppUser> user, List<Comment> comments);
    }
}
