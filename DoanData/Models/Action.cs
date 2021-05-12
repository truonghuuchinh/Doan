using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DoanData.Models
{

    public class Action
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FunctionsId { get; set; }
        public virtual Function function { get;set; }
        public List<UserRole> UserRoles { get; set; }
    }
}
