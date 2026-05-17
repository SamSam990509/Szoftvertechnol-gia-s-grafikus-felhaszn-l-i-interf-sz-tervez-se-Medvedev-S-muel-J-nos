using SemesterProject.Models;
using SemesterProject.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SemesterProject
{
    public partial class ModifyRecipeWindow : Window
    {
        private MainViewModel viewModel;
        private Recipe? selectedRecipe;
        private ObservableCollection<RecipeIngredient> editedIngredients;

        public ModifyRecipeWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            viewModel = mainViewModel;
            editedIngredients = new ObservableCollection<RecipeIngredient>();

            RecipeComboBox.ItemsSource = viewModel.Recipes;

            if (viewModel.SelectedRecipe != null)
            {
                RecipeComboBox.SelectedItem = viewModel.SelectedRecipe;
            }

            IngredientsListBox.ItemsSource = editedIngredients;
        }

        private void RecipeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecipe = RecipeComboBox.SelectedItem as Recipe;

            if (selectedRecipe == null)
            {
                return;
            }

            RecipeNameTextBox.Text = selectedRecipe.Name;

            editedIngredients.Clear();

            foreach (RecipeIngredient ingredient in selectedRecipe.Ingredients)
            {
                editedIngredients.Add(new RecipeIngredient(
                    ingredient.FoodName,
                    ingredient.RequiredQuantity));
            }

            IngredientsListBox.Items.Refresh();
        }

        private void IngredientsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecipeIngredient selectedIngredient = IngredientsListBox.SelectedItem as RecipeIngredient;

            if (selectedIngredient != null)
            {
                IngredientNameTextBox.Text = selectedIngredient.FoodName;
                IngredientQuantityTextBox.Text = selectedIngredient.RequiredQuantity.ToString();
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe first.");
                return;
            }

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

            RecipeIngredient existingIngredient = editedIngredients.FirstOrDefault(
                ingredient => ingredient.FoodName.Equals(ingredientName, StringComparison.OrdinalIgnoreCase));

            if (existingIngredient == null)
            {
                editedIngredients.Add(new RecipeIngredient(ingredientName, quantity));
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

            editedIngredients.Remove(selectedIngredient);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe first.");
                return;
            }

            string recipeName = RecipeNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(recipeName))
            {
                MessageBox.Show("Please enter a recipe name.");
                return;
            }

            bool nameUsedByOtherRecipe = viewModel.Recipes.Any(
                recipe => recipe != selectedRecipe &&
                          recipe.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));

            if (nameUsedByOtherRecipe)
            {
                MessageBox.Show("Another recipe already has this name.");
                return;
            }

            if (editedIngredients.Count == 0)
            {
                MessageBox.Show("A recipe must have at least one ingredient.");
                return;
            }

            selectedRecipe.Name = recipeName;
            selectedRecipe.Ingredients.Clear();

            foreach (RecipeIngredient ingredient in editedIngredients)
            {
                selectedRecipe.Ingredients.Add(new RecipeIngredient(
                    ingredient.FoodName,
                    ingredient.RequiredQuantity));
            }

            viewModel.SelectedRecipe = selectedRecipe;

            CollectionViewSource.GetDefaultView(viewModel.Recipes).Refresh();

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