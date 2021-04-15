using System.Collections.Generic;

namespace DoanData.Models
{
    public class Function
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public List<Action> ActionList { get; set; }
    }
}