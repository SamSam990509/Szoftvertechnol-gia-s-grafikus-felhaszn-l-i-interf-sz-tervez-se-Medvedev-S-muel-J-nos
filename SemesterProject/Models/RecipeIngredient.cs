using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace SemesterProject.Models
{
    public class RecipeIngredient : INotifyPropertyChanged
    {
        public string FoodName { get; set; }

        private int requiredQuantity;

        public int RequiredQuantity
        {
            get { return requiredQuantity; }
            set
            {
                requiredQuantity = value;
                OnPropertyChanged(nameof(RequiredQuantity));
            }
        }

        public RecipeIngredient(string foodName, int requiredQuantity)
        {
            FoodName = foodName;
            RequiredQuantity = requiredQuantity;
        }

        public override string ToString()
        {
            return $"{FoodName} - required: {RequiredQuantity}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
