using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class FunctionRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên chức năng!")]
        public string Name { get; set; }
    }
}
