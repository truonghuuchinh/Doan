using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class UserRoleRequest
    {
        public UserRoleRequest(string RoleName,int UserId)
        {
            this.RoleName = RoleName;
            this.UserId = UserId;
        }  
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
    }
}
