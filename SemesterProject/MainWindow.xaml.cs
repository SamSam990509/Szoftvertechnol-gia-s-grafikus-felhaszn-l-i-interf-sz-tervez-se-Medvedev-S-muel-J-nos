using SemesterProject.ViewModels;
using System.Windows;

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
        }

        private void HideAllPages()
        {
            MainPagePanel.Visibility = Visibility.Collapsed;
            FridgePagePanel.Visibility = Visibility.Collapsed;
            MembersPagePanel.Visibility = Visibility.Collapsed;
        }

        private void BackToMainPageButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            MainPagePanel.Visibility = Visibility.Visible;
        }

        private void FridgeContentsButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            FridgePagePanel.Visibility = Visibility.Visible;
        }

        private void HouseholdMembersButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            MembersPagePanel.Visibility = Visibility.Visible;
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

            viewModel.AddFood(foodName, quantity);

            FoodNameTextBox.Text = "";
            FoodQuantityTextBox.Text = "";

            MessageBox.Show("Food added successfully.");
        }

        private void RemoveFoodButton_Click(object sender, RoutedEventArgs e)
        {
            string foodName = RemoveFoodNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(foodName) || foodName == "Food name")
            {
                MessageBox.Show("Please enter a food name.");
                return;
            }

            if (!int.TryParse(RemoveFoodQuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            bool success = viewModel.RemoveFood(foodName, quantity);

            RemoveFoodNameTextBox.Text = "";
            RemoveFoodQuantityTextBox.Text = "";

            if (success)
            {
                MessageBox.Show("Food removed successfully.");
            }
            else
            {
                MessageBox.Show("This food is not in the fridge.");
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
            string memberName = RemoveMemberNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(memberName) || memberName == "Member name")
            {
                MessageBox.Show("Please enter the member name.");
                return;
            }

            bool success = viewModel.RemoveMember(memberName);

            RemoveMemberNameTextBox.Text = "";

            if (success)
            {
                MessageBox.Show("Member removed successfully.");
            }
            else
            {
                MessageBox.Show("This member does not exist.");
            }
        }
    }
}