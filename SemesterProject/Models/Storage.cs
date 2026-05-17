using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class Storage
    {
        public string Name { get; set; }
        public int Capacity { get; set; }

        public Storage(string name, int capacity)
        {
            Name = name;
            Capacity = capacity;
        }

        public override string ToString()
        {
            return $"{Name} - Capacity: {Capacity}";
        }
    }
}
