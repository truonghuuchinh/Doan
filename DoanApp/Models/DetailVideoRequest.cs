using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class DetailVideoRequest
    {
        public int Id { get; set; }
       
        public int PlayListId { get; set; }
        public int VideoId { get; set; }
    }
}
