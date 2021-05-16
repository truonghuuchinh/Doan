using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreateDate { get; set; }
        public bool Status { get; set; } = true;
        public List<Video> VideoList { get; set; }
    }
}
