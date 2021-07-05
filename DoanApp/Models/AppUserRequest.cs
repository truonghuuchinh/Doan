using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class AppUserRequest
    {
        public int Id { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 kí tự,1 số và 1 kí tự đặc biệt!")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        public string PasswordHash { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage ="Trường này phải là Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập trường này!")]
        public string FirtsName { get; set; }
        public bool LoginExternal { get; set; } = false;
        [Required(ErrorMessage = "Vui lòng nhập trường này!")]
        public string LastName { get; set; }
        public string CreateDate { get; set; }
        public string DescriptionChannel { get; set; }
        public string Avartar { get; set; }
        public string SecurityStamp { get; set; }
        public bool RememberMe { get; set; }
    }
}
