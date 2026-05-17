using SemesterProject.Models;
using SemesterProject.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SemesterProject
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            DataContext = viewModel;

            RefreshStorageUsage();
        }

        private void HideAllPages()
        {
            MainPagePanel.Visibility = Visibility.Collapsed;
            FridgePagePanel.Visibility = Visibility.Collapsed;
            MembersPagePanel.Visibility = Visibility.Collapsed;
            RecipesPagePanel.Visibility = Visibility.Collapsed;
        }

        private void BackToMainPageButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            MainPagePanel.Visibility = Visibility.Visible;
            RefreshStorageUsage();
        }

        private void FridgeContentsButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            FridgePagePanel.Visibility = Visibility.Visible;
            RefreshStorageUsage();
        }

        private void HouseholdMembersButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            MembersPagePanel.Visibility = Visibility.Visible;
        }

        private void RecipesButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            RecipesPagePanel.Visibility = Visibility.Visible;
            RefreshStorageUsage();
            RefreshRecipeAvailability();
        }

        private void RefreshStorageUsage()
        {
            StorageUsageTextBlock.Text = viewModel.GetStorageUsageText();
        }

        private string GetSelectedPreparedFoodStorage()
        {
            if (StoreInFreezerRadioButton.IsChecked == true)
            {
                return "Freezer";
            }

            if (StoreInPantryRadioButton.IsChecked == true)
            {
                return "Pantry";
            }

            return "Fridge";
        }

        private void FoodListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FoodItem selectedFood = FoodListBox.SelectedItem as FoodItem;

            if (selectedFood != null)
            {
                RemoveFoodNameTextBox.Text = $"{selectedFood.Name} ({selectedFood.StorageLocation})";
            }
        }

        private void MembersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HouseholdMember selectedMember = MembersListBox.SelectedItem as HouseholdMember;

            if (selectedMember != null)
            {
                RemoveMemberNameTextBox.Text = selectedMember.Name;
            }
        }

        private void AddFoodButton_Click(object sender, RoutedEventArgs e)
        {
            string foodName = FoodNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(foodName) || foodName == "Food name")
            {
                MessageBox.Show("Please enter a food name.");
                return;
            }

            if (!int.TryParse(FoodQuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            string storageLocation = StorageLocationComboBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(storageLocation))
            {
                MessageBox.Show("Please select a storage location.");
                return;
            }

            bool success = viewModel.AddFood(foodName, quantity, storageLocation, out string message);

            FoodNameTextBox.Text = "";
            FoodQuantityTextBox.Text = "";

            FoodListBox.Items.Refresh();
            RefreshStorageUsage();
            RefreshRecipeAvailability();

            MessageBox.Show(message);
        }

        private void RemoveFoodButton_Click(object sender, RoutedEventArgs e)
        {
            FoodItem selectedFood = FoodListBox.SelectedItem as FoodItem;

            if (selectedFood == null)
            {
                MessageBox.Show("Please select a food item from the list.");
                return;
            }

            if (!int.TryParse(RemoveFoodQuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity to remove.");
                return;
            }

            bool success = viewModel.RemoveFood(selectedFood, quantity);

            RemoveFoodNameTextBox.Text = "Click an item from the list";
            RemoveFoodQuantityTextBox.Text = "Quantity to remove";
            FoodListBox.SelectedItem = null;

            FoodListBox.Items.Refresh();
            RefreshStorageUsage();
            RefreshRecipeAvailability();

            if (success)
            {
                MessageBox.Show("Food quantity was updated successfully.");
            }
            else
            {
                MessageBox.Show("Food could not be removed.");
            }
        }

        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            string memberName = MemberNameTextBox.Text.Trim();
            string favoriteFood = FavoriteFoodTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(memberName) || memberName == "Member name")
            {
                MessageBox.Show("Please enter the member name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(favoriteFood) || favoriteFood == "Favorite food")
            {
                MessageBox.Show("Please enter the favorite food.");
                return;
            }

            bool success = viewModel.AddMember(memberName, favoriteFood);

            MemberNameTextBox.Text = "";
            FavoriteFoodTextBox.Text = "";

            if (success)
            {
                MessageBox.Show("Member added successfully.");
            }
            else
            {
                MessageBox.Show("This member already exists.");
            }
        }

        private void RemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            HouseholdMember selectedMember = MembersListBox.SelectedItem as HouseholdMember;

            if (selectedMember == null)
            {
                MessageBox.Show("Please select a member from the list.");
                return;
            }

            bool success = viewModel.RemoveMember(selectedMember.Name);

            RemoveMemberNameTextBox.Text = "Click a member from the list";
            MembersListBox.SelectedItem = null;

            if (success)
            {
                MessageBox.Show("Member removed successfully.");
            }
            else
            {
                MessageBox.Show("This member does not exist.");
            }
        }

        private void RefreshRecipeAvailabilityButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshRecipeAvailability();
        }

        private void MakeRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            string storageLocation = GetSelectedPreparedFoodStorage();

            bool success = viewModel.MakeSelectedRecipe(storageLocation, out string message);

            FoodListBox.Items.Refresh();
            RefreshStorageUsage();
            RefreshRecipeAvailability();

            MessageBox.Show(message);
        }

        private void AddRecipeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            AddRecipeWindow addRecipeWindow = new AddRecipeWindow(viewModel);
            addRecipeWindow.Owner = this;

            bool? result = addRecipeWindow.ShowDialog();

            if (result == true)
            {
                RefreshRecipeAvailability();
                MessageBox.Show("Recipe was saved successfully.");
            }
        }

        private void ModifyRecipeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyRecipeWindow modifyRecipeWindow = new ModifyRecipeWindow(viewModel);
            modifyRecipeWindow.Owner = this;

            bool? result = modifyRecipeWindow.ShowDialog();

            if (result == true)
            {
                RefreshRecipeAvailability();
                MessageBox.Show("Recipe was modified successfully.");
            }
        }

        private void RefreshRecipeAvailability()
        {
            RecipeAvailabilityStackPanel.Children.Clear();

            if (viewModel.SelectedRecipe == null)
            {
                return;
            }

            foreach (RecipeIngredient ingredient in viewModel.SelectedRecipe.Ingredients)
            {
                string status = viewModel.GetIngredientStatus(ingredient);

                Brush backgroundColor;

                if (status == "Enough")
                {
                    backgroundColor = Brushes.LightGreen;
                }
                else if (status == "Not enough")
                {
                    backgroundColor = Brushes.Khaki;
                }
                else
                {
                    backgroundColor = Brushes.LightCoral;
                }

                Border border = new Border
                {
                    Background = backgroundColor,
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 5, 0, 5)
                };

                TextBlock textBlock = new TextBlock
                {
                    Text = $"{ingredient.FoodName} - required: {ingredient.RequiredQuantity} - status: {status}",
                    FontSize = 15,
                    FontWeight = FontWeights.SemiBold
                };

                border.Child = textBlock;
                RecipeAvailabilityStackPanel.Children.Add(border);
            }
        }
    }
}