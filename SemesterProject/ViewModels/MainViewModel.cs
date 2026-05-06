using SemesterProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using SemesterProject.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SemesterProject.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FoodItem> FridgeFoods { get; set; }
        public ObservableCollection<HouseholdMember> HouseholdMembers { get; set; }

        private string alertMessage;

        public string AlertMessage
        {
            get { return alertMessage; }
            set
            {
                alertMessage = value;
                OnPropertyChanged(nameof(AlertMessage));
            }
        }

        public MainViewModel()
        {
            FridgeFoods = new ObservableCollection<FoodItem>
            {
                new FoodItem("Milk", 2),
                new FoodItem("Cheese", 1),
                new FoodItem("Eggs", 10),
                new FoodItem("Yogurt", 3)
            };

            HouseholdMembers = new ObservableCollection<HouseholdMember>
            {
                new HouseholdMember("Anna", "Milk"),
                new HouseholdMember("Peter", "Pizza"),
                new HouseholdMember("David", "Cheese")
            };

            UpdateFavoriteFoodAlerts();
        }

        public void AddFood(string foodName, int quantity)
        {
            FoodItem existingFood = FridgeFoods.FirstOrDefault(
                food => food.Name.Equals(foodName, StringComparison.OrdinalIgnoreCase));

            if (existingFood == null)
            {
                FridgeFoods.Add(new FoodItem(foodName, quantity));
            }
            else
            {
                existingFood.Quantity += quantity;
            }

            UpdateFavoriteFoodAlerts();
        }

        public void RemoveFood(string foodName, int quantity)
        {
            FoodItem existingFood = FridgeFoods.FirstOrDefault(
                food => food.Name.Equals(foodName, StringComparison.OrdinalIgnoreCase));

            if (existingFood == null)
            {
                return;
            }

            existingFood.Quantity -= quantity;

            if (existingFood.Quantity <= 0)
            {
                FridgeFoods.Remove(existingFood);
            }

            UpdateFavoriteFoodAlerts();
        }

        public void AddMember(string memberName, string favoriteFood)
        {
            HouseholdMember existingMember = HouseholdMembers.FirstOrDefault(
                member => member.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));

            if (existingMember == null)
            {
                HouseholdMembers.Add(new HouseholdMember(memberName, favoriteFood));
            }

            UpdateFavoriteFoodAlerts();
        }

        public void RemoveMember(string memberName)
        {
            HouseholdMember existingMember = HouseholdMembers.FirstOrDefault(
                member => member.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));

            if (existingMember != null)
            {
                HouseholdMembers.Remove(existingMember);
            }

            UpdateFavoriteFoodAlerts();
        }

        public void UpdateFavoriteFoodAlerts()
        {
            var missingFavoriteFoods = HouseholdMembers
                .Where(member => !FridgeFoods.Any(food =>
                    food.Name.Equals(member.FavoriteFood, StringComparison.OrdinalIgnoreCase)
                    && food.Quantity > 0))
                .Select(member => $"{member.Name}'s favorite food is out of stock: {member.FavoriteFood}")
                .ToList();

            if (missingFavoriteFoods.Count == 0)
            {
                AlertMessage = "All favorite foods are available.";
            }
            else
            {
                AlertMessage = string.Join("\n", missingFavoriteFoods);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
