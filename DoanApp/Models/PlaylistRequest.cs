using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class PlaylistRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string CreateDate { get; set; }
        public bool Status { get; set; } = true;
    }
}
