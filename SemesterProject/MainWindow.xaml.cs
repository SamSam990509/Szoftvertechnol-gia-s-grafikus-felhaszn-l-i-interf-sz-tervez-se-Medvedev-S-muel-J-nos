using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SemesterProject.ViewModels;


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
            AddFoodPagePanel.Visibility = Visibility.Collapsed;
            RemoveFoodPagePanel.Visibility = Visibility.Collapsed;
            AddMemberPagePanel.Visibility = Visibility.Collapsed;
            RemoveMemberPagePanel.Visibility = Visibility.Collapsed;
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

        private void ShowAddFoodButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            AddFoodPagePanel.Visibility = Visibility.Visible;
        }

        private void ShowRemoveFoodButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            RemoveFoodPagePanel.Visibility = Visibility.Visible;
        }

        private void ShowAddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            AddMemberPagePanel.Visibility = Visibility.Visible;
        }

        private void ShowRemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            HideAllPages();
            RemoveMemberPagePanel.Visibility = Visibility.Visible;
        }
        private void AddFoodButton_Click(object sender, RoutedEventArgs e)
        {
            string foodName = FoodNameTextBox.Text.Trim();

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

            if (!int.TryParse(RemoveFoodQuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            viewModel.RemoveFood(foodName, quantity);

            RemoveFoodNameTextBox.Text = "";
            RemoveFoodQuantityTextBox.Text = "";

            MessageBox.Show("Food removed successfully.");
        }

        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            string memberName = MemberNameTextBox.Text.Trim();
            string favoriteFood = FavoriteFoodTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(memberName) || string.IsNullOrWhiteSpace(favoriteFood))
            {
                MessageBox.Show("Please enter the member name and favorite food.");
                return;
            }

            viewModel.AddMember(memberName, favoriteFood);

            MemberNameTextBox.Text = "";
            FavoriteFoodTextBox.Text = "";

            MessageBox.Show("Member added successfully.");
        }

        private void RemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            string memberName = RemoveMemberNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(memberName))
            {
                MessageBox.Show("Please enter the member name.");
                return;
            }

            viewModel.RemoveMember(memberName);

            RemoveMemberNameTextBox.Text = "";

            MessageBox.Show("Member removed successfully.");
        }
    }

}