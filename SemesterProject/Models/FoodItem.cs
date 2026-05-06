using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class FoodItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }

        public FoodItem(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Name} - Quantity: {Quantity}";
        }
    }
}
