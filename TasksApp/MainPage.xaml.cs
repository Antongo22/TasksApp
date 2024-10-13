using TasksApp.Pages;

namespace TasksApp
{
    public partial class MainPage : ContentPage
    {
        MainContentView MainContentView;
        CreateContentView CreateContentView;
        SettingsContentView SettingsContentView;

        public MainPage()
        {
            InitializeComponent();
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            MainContentView = new();
            CreateContentView = new();
            SettingsContentView = new();

            MainFrame.Content = MainContentView;
        }

        private void MainPageButton_Clicked(object sender, EventArgs e)
        {
            MainFrame.Content = MainContentView;
        }

        private void CreatePageButton_Clicked(object sender, EventArgs e)
        {
            MainFrame.Content = CreateContentView;
        }

        private void SettingsPageButton_Clicked(object sender, EventArgs e)
        {
            MainFrame.Content = SettingsContentView;
        }
    }

}
