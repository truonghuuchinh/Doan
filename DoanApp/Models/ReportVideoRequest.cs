using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class ReportVideoRequest
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }
        public int VideoId { get; set; }
    }
}
