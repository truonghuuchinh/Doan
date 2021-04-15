using System;
using System.Collections.Generic;

namespace DoanData.Models
{
    public class PlayList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreateDate { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appuser {get;set;}
        public List<DetailVideo> Details { get; set; }
    }
}