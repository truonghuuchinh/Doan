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
    public class UserRoleService : IUserRoleService
    {
        private readonly DpContext _context;
        public UserRoleService(DpContext context)
        {
            _context = context;
        }
        public async Task<int> Create(UserRoleRequest roleRequest)
        {
            var role = new UserRole();
            role.Id = roleRequest.Id;
            role.RoleName = roleRequest.RoleName;
            _context.UserRole.Add(role);
            return await  _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int id)
        {
            var role = _context.UserRole.FirstOrDefault(x => x.Id == id);
            _context.Remove(role);
           return await _context.SaveChangesAsync();
        }

        public async Task<List<UserRole>> GetAll()
        {
            return await _context.UserRole.ToListAsync();
        }

        public async Task<int> Update(UserRoleRequest roleRequest)
        {
            var role = _context.UserRole.FirstOrDefault(x => x.Id ==roleRequest.Id);
            role.RoleName = roleRequest.RoleName;
            _context.Update(role);
            return await _context.SaveChangesAsync();
        }
    }
}
