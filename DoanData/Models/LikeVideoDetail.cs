using DoanData.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
   public class LikeVideoDetail
    {
        public int Id { get; set; }
        public Reaction Reaction { get; set; }
        public string CreateDate { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appuser { get; set; }
    }
}
