using SemesterProject.Models;
using SemesterProject.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SemesterProject
{
    public partial class AddRecipeWindow : Window
    {
        private MainViewModel viewModel;
        private ObservableCollection<RecipeIngredient> ingredients;

        public AddRecipeWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            viewModel = mainViewModel;
            ingredients = new ObservableCollection<RecipeIngredient>();
            IngredientsListBox.ItemsSource = ingredients;
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            string ingredientName = IngredientNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(ingredientName) || ingredientName == "Ingredient name")
            {
                MessageBox.Show("Please enter an ingredient name.");
                return;
            }

            if (!int.TryParse(IngredientQuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            RecipeIngredient existingIngredient = ingredients.FirstOrDefault(
                ingredient => ingredient.FoodName.Equals(ingredientName, StringComparison.OrdinalIgnoreCase));

            if (existingIngredient == null)
            {
                ingredients.Add(new RecipeIngredient(ingredientName, quantity));
            }
            else
            {
                existingIngredient.RequiredQuantity = quantity;
                IngredientsListBox.Items.Refresh();
            }

            IngredientNameTextBox.Text = "";
            IngredientQuantityTextBox.Text = "";
        }

        private void RemoveIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            RecipeIngredient selectedIngredient = IngredientsListBox.SelectedItem as RecipeIngredient;

            if (selectedIngredient == null)
            {
                MessageBox.Show("Please select an ingredient to remove.");
                return;
            }

            ingredients.Remove(selectedIngredient);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = RecipeNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(recipeName))
            {
                MessageBox.Show("Please enter a recipe name.");
                return;
            }

            bool alreadyExists = viewModel.Recipes.Any(
                recipe => recipe.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                MessageBox.Show("This recipe already exists.");
                return;
            }

            if (ingredients.Count == 0)
            {
                MessageBox.Show("Please add at least one ingredient.");
                return;
            }

            Recipe newRecipe = new Recipe(recipeName);

            foreach (RecipeIngredient ingredient in ingredients)
            {
                newRecipe.Ingredients.Add(new RecipeIngredient(
                    ingredient.FoodName,
                    ingredient.RequiredQuantity));
            }

            viewModel.Recipes.Add(newRecipe);
            viewModel.SelectedRecipe = newRecipe;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}