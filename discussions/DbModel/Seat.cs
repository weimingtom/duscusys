//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Discussions.DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Seat
    {
        public Seat()
        {
            this.Person = new HashSet<Person>();
        }
    
        public int Id { get; set; }
        public int Color { get; set; }
        public string SeatName { get; set; }
    
        public virtual ICollection<Person> Person { get; set; }
    }
}