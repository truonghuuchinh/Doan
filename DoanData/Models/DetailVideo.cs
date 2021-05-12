using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoanData.Models
{
    public class DetailVideo
    {
        public int Id { get; set; }
        public int PlayListId { get; set; }
        public virtual PlayList playList { get; set; }

        [ForeignKey("VideoId")]
        public int VideoId { get; set; }
        public virtual Video video { get; set; }
    }
}
