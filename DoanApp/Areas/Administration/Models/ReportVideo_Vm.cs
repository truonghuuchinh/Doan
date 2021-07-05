using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Models
{
    public class ReportVideo_Vm
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string NameVideo { get; set; }
        public string ImgPoster { get; set; }
        public int VideoId { get; set; }
        public string CreateDate { get; set; }
        public int UserId { get; set; }
        public string NamUser { get; set; }
    }
}
