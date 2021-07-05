using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Models
{
    public class UserAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string  CreateDate { get; set; }
        public string Avartar { get; set; }
        public bool LoginExternal { get; set; }
        public bool LockOutEnabled { get; set; }
        public int TotalVideo { get; set; }
        public int TotalView { get; set; }
    }
}
