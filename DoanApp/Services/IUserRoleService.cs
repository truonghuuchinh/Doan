using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IUserRoleService
    {
        Task<List<UserRole>> GetAll();
        Task<int> Create(UserRoleRequest roleRequest);
        Task<int> Update(UserRoleRequest roleRequest);
        Task<int> Delete(int id);
    }
}
