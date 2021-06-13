using DoanData.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class Video_vm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LinkVideo { get; set; }
        public int Like { get; set; }
        public string PosterImg { get; set; }
        public int DisLike { get; set; }
        public int ViewCount { get; set; }
        public bool HidenVideo { get; set; }
        public int CategorysId { get; set; }
        public int AppUserId { get; set; }
        public bool Status { get; set; } = true;
        public string Avartar { get; set; }
        public string FirtsName { get; set; }
        public int UserLike { get; set; }
        public string ImgChannel { get; set; }
        public bool LoginExternal { get; set; }
        public string LastName { get; set; }
        public string Reaction { get; set; } = Reactions.NoAction.ToString();
        public string CreateDate { get; set; }
    }
}
