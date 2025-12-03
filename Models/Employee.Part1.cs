using System;
using System.Collections.Generic;

namespace Models
{
    public partial class Employee : Person
    {
        public string Position { get; set; }
        public List<int> TaskIds { get; set; } = new List<int>();

        public Employee() { }

        public Employee(int id, string firstName, string lastName, string position)
            : base(id, firstName, lastName)
        {
            Position = position;
        }
    }
}
