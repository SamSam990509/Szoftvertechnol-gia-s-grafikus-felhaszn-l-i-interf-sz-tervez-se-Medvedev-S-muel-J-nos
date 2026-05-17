using SemesterProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SemesterProject.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FoodItem> FridgeFoods { get; set; }
        public ObservableCollection<HouseholdMember> HouseholdMembers { get; set; }
        public ObservableCollection<Recipe> Recipes { get; set; }
        public ObservableCollection<Storage> Storages { get; set; }
        public ObservableCollection<string> StorageLocations { get; set; }

        private Dictionary<string, string> foodStorageRules;

        private string alertMessage = "";
        private Recipe? selectedRecipe;

        public string AlertMessage
        {
            get { return alertMessage; }
            set
            {
                alertMessage = value;
                OnPropertyChanged(nameof(AlertMessage));
            }
        }

        public Recipe? SelectedRecipe
        {
            get { return selectedRecipe; }
            set
            {
                selectedRecipe = value;
                OnPropertyChanged(nameof(SelectedRecipe));
                OnPropertyChanged(nameof(SelectedRecipeIngredients));
            }
        }

        public ObservableCollection<RecipeIngredient> SelectedRecipeIngredients
        {
            get
            {
                if (SelectedRecipe == null)
                {
                    return new ObservableCollection<RecipeIngredient>();
                }

                return SelectedRecipe.Ingredients;
            }
        }

        public MainViewModel()
        {
            StorageLocations = new ObservableCollection<string>
            {
                "Fridge",
                "Freezer",
                "Pantry"
            };

            Storages = new ObservableCollection<Storage>
            {
                new Storage("Fridge", 20),
                new Storage("Freezer", 15),
                new Storage("Pantry", 50)
            };

            foodStorageRules = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Milk", "Fridge" },
                { "Cheese", "Fridge" },
                { "Yogurt", "Fridge" },
                { "Eggs", "Fridge" },
                { "Butter", "Fridge" },
                { "Chicken", "Fridge" },

                { "Ice cream", "Freezer" },
                { "Frozen pizza", "Freezer" },
                { "Frozen vegetables", "Freezer" },
                { "Frozen meat", "Freezer" },

                { "Flour", "Pantry" },
                { "Rice", "Pantry" },
                { "Pasta", "Pantry" },
                { "Sugar", "Pantry" },
                { "Cereal", "Pantry" },
                { "Bread", "Pantry" }
            };

            FridgeFoods = new ObservableCollection<FoodItem>
            {
                new FoodItem("Milk", 2, "Fridge"),
                new FoodItem("Cheese", 1, "Fridge"),
                new FoodItem("Eggs", 10, "Fridge"),
                new FoodItem("Yogurt", 3, "Fridge"),
                new FoodItem("Ice cream", 2, "Freezer"),
                new FoodItem("Flour", 1, "Pantry"),
                new FoodItem("Rice", 3, "Pantry")
            };

            HouseholdMembers = new ObservableCollection<HouseholdMember>
            {
                new HouseholdMember("Anna", "Milk"),
                new HouseholdMember("Peter", "Pizza"),
                new HouseholdMember("David", "Cheese")
            };

            Recipes = new ObservableCollection<Recipe>();

            Recipe pancake = new Recipe("Pancake");
            pancake.Ingredients.Add(new RecipeIngredient("Milk", 1));
            pancake.Ingredients.Add(new RecipeIngredient("Eggs", 2));
            pancake.Ingredients.Add(new RecipeIngredient("Flour", 1));

            Recipes.Add(pancake);
            SelectedRecipe = pancake;

            UpdateFavoriteFoodAlerts();
        }

        public string GetStorageUsageText()
        {
            string result = "";

            foreach (Storage storage in Storages)
            {
                int usedAmount = GetStorageUsedAmount(storage.Name);
                result += $"{storage.Name}: {usedAmount} / {storage.Capacity} used\n";
            }

            return result.Trim();
        }

        public int GetStorageUsedAmount(string storageLocation)
        {
            return FridgeFoods
                .Where(food => food.StorageLocation.Equals(storageLocation, StringComparison.OrdinalIgnoreCase))
                .Sum(food => food.Quantity);
        }

        public int GetStorageCapacity(string storageLocation)
        {
            Storage? storage = Storages.FirstOrDefault(
                item => item.Name.Equals(storageLocation, StringComparison.OrdinalIgnoreCase));

            if (storage == null)
            {
                return 0;
            }

            return storage.Capacity;
        }

        public bool IsCorrectStorage(string foodName, string storageLocation, out string message)
        {
            if (foodStorageRules.ContainsKey(foodName))
            {
                string correctStorage = foodStorageRules[foodName];

                if (!correctStorage.Equals(storageLocation, StringComparison.OrdinalIgnoreCase))
                {
                    message = $"{foodName} cannot be stored in {storageLocation}. Suggested storage: {correctStorage}.";
                    return false;
                }
            }

            message = "";
            return true;
        }

        public bool HasEnoughStorageSpace(string storageLocation, int quantityToAdd, out string message)
        {
            Storage? storage = Storages.FirstOrDefault(
                item => item.Name.Equals(storageLocation, StringComparison.OrdinalIgnoreCase));

            if (storage == null)
            {
                message = "Storage location was not found.";
                return false;
            }

            int usedAmount = GetStorageUsedAmount(storageLocation);

            if (usedAmount + quantityToAdd > storage.Capacity)
            {
                message =
                    $"There is not enough space in {storageLocation}.\n" +
                    $"Capacity: {storage.Capacity}\n" +
                    $"Current amount: {usedAmount}\n" +
                    $"Trying to add: {quantityToAdd}";

                return false;
            }

            message = "";
            return true;
        }

        public bool AddFood(string foodName, int quantity, string storageLocation, out string message)
        {
            if (!IsCorrectStorage(foodName, storageLocation, out message))
            {
                return false;
            }

            if (!HasEnoughStorageSpace(storageLocation, quantity, out message))
            {
                return false;
            }

            FoodItem? existingFood = FridgeFoods.FirstOrDefault(
                food => food.Name.Equals(foodName, StringComparison.OrdinalIgnoreCase)
                        && food.StorageLocation.Equals(storageLocation, StringComparison.OrdinalIgnoreCase));

            if (existingFood == null)
            {
                FridgeFoods.Add(new FoodItem(foodName, quantity, storageLocation));
            }
            else
            {
                existingFood.Quantity += quantity;
            }

            UpdateFavoriteFoodAlerts();
            OnPropertyChanged(nameof(FridgeFoods));

            message = "Food added successfully.";
            return true;
        }

        public bool RemoveFood(FoodItem selectedFood, int quantity)
        {
            if (selectedFood == null)
            {
                return false;
            }

            selectedFood.Quantity -= quantity;

            if (selectedFood.Quantity <= 0)
            {
                FridgeFoods.Remove(selectedFood);
            }

            UpdateFavoriteFoodAlerts();
            OnPropertyChanged(nameof(FridgeFoods));

            return true;
        }

        public bool AddMember(string memberName, string favoriteFood)
        {
            HouseholdMember? existingMember = HouseholdMembers.FirstOrDefault(
                member => member.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));

            if (existingMember != null)
            {
                return false;
            }

            HouseholdMembers.Add(new HouseholdMember(memberName, favoriteFood));

            UpdateFavoriteFoodAlerts();
            return true;
        }

        public bool RemoveMember(string memberName)
        {
            HouseholdMember? existingMember = HouseholdMembers.FirstOrDefault(
                member => member.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));

            if (existingMember == null)
            {
                return false;
            }

            HouseholdMembers.Remove(existingMember);

            UpdateFavoriteFoodAlerts();
            return true;
        }

        public string GetIngredientStatus(RecipeIngredient ingredient)
        {
            FoodItem? existingFood = FridgeFoods.FirstOrDefault(
                food => food.Name.Equals(ingredient.FoodName, StringComparison.OrdinalIgnoreCase));

            if (existingFood == null)
            {
                return "Missing";
            }

            if (existingFood.Quantity >= ingredient.RequiredQuantity)
            {
                return "Enough";
            }

            return "Not enough";
        }

        public int GetRecipeAvailabilityPercentage()
        {
            if (SelectedRecipe == null || SelectedRecipe.Ingredients.Count == 0)
            {
                return 0;
            }

            int enoughIngredients = 0;

            foreach (RecipeIngredient ingredient in SelectedRecipe.Ingredients)
            {
                string status = GetIngredientStatus(ingredient);

                if (status == "Enough")
                {
                    enoughIngredients++;
                }
            }

            double percentage = (double)enoughIngredients / SelectedRecipe.Ingredients.Count * 100;
            return (int)Math.Round(percentage);
        }

        public string GetRecipeAvailabilityText()
        {
            if (SelectedRecipe == null)
            {
                return "No recipe selected.";
            }

            int percentage = GetRecipeAvailabilityPercentage();
            return $"{SelectedRecipe.Name} availability: {percentage}%";
        }

        public bool CanMakeSelectedRecipe(out string message)
        {
            if (SelectedRecipe == null)
            {
                message = "Please select a recipe first.";
                return false;
            }

            List<string> missingItems = new List<string>();

            foreach (RecipeIngredient ingredient in SelectedRecipe.Ingredients)
            {
                FoodItem? existingFood = FridgeFoods.FirstOrDefault(
                    food => food.Name.Equals(ingredient.FoodName, StringComparison.OrdinalIgnoreCase));

                if (existingFood == null)
                {
                    missingItems.Add($"{ingredient.FoodName}: required {ingredient.RequiredQuantity}, available 0");
                }
                else if (existingFood.Quantity < ingredient.RequiredQuantity)
                {
                    missingItems.Add($"{ingredient.FoodName}: required {ingredient.RequiredQuantity}, available {existingFood.Quantity}");
                }
            }

            if (missingItems.Count > 0)
            {
                message = "Cannot make recipe.\nMissing or insufficient ingredients:\n" + string.Join("\n", missingItems);
                return false;
            }

            message = "";
            return true;
        }

        public bool MakeSelectedRecipe(string storageLocation, out string message)
        {
            if (SelectedRecipe == null)
            {
                message = "Please select a recipe first.";
                return false;
            }

            if (!CanMakeSelectedRecipe(out message))
            {
                return false;
            }

            if (!HasEnoughStorageSpace(storageLocation, 1, out message))
            {
                message = "The recipe can be made, but there is not enough space to store the prepared food.\n" + message;
                return false;
            }

            foreach (RecipeIngredient ingredient in SelectedRecipe.Ingredients.ToList())
            {
                FoodItem existingFood = FridgeFoods.First(
                    food => food.Name.Equals(ingredient.FoodName, StringComparison.OrdinalIgnoreCase));

                existingFood.Quantity -= ingredient.RequiredQuantity;

                if (existingFood.Quantity <= 0)
                {
                    FridgeFoods.Remove(existingFood);
                }
            }

            FoodItem? preparedFood = FridgeFoods.FirstOrDefault(
                food => food.Name.Equals(SelectedRecipe.Name, StringComparison.OrdinalIgnoreCase)
                        && food.StorageLocation.Equals(storageLocation, StringComparison.OrdinalIgnoreCase));

            if (preparedFood == null)
            {
                FridgeFoods.Add(new FoodItem(SelectedRecipe.Name, 1, storageLocation));
            }
            else
            {
                preparedFood.Quantity += 1;
            }

            UpdateFavoriteFoodAlerts();
            OnPropertyChanged(nameof(FridgeFoods));

            message = $"You made {SelectedRecipe.Name} successfully. It was stored in {storageLocation}.";
            return true;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}