using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class HouseholdMember
    {
        public string Name { get; set; }
        public string FavoriteFood { get; set; }

        public HouseholdMember(string name, string favoriteFood)
        {
            Name = name;
            FavoriteFood = favoriteFood;
        }

        public override string ToString()
        {
            return $"{Name} - Favorite food: {FavoriteFood}";
        }
    }
}
