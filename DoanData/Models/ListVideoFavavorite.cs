using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
    public class ListVideoFavavorite
    {
        public int Id { get; set; }
        public string CreateDate { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appUser { get; set; }
    }
}
