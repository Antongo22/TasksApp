using TasksApp.Pages;

namespace TasksApp
{
    public partial class MainPage : ContentPage
    {
        public MainContentView MainContentView;
        public CreateContentView CreateContentView;
        public SettingsContentView SettingsContentView;

        public MainPage()
        {
            InitializeComponent();
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {

            MainContentView = new();
            CreateContentView = new(this);
            SettingsContentView = new();


            MainFrame.Content = MainContentView;
        }

        private void MainPageButton_Clicked(object sender, EventArgs e)
        {
            Title = "Задачи";
            MainFrame.Content = MainContentView;
        }

        public void SetMainContentView()
        {
            MainFrame.Content = MainContentView;
        }


        private void CreatePageButton_Clicked(object sender, EventArgs e)
        {
            Title = "Создать задачу";
            MainFrame.Content = CreateContentView;
        }

        private void SettingsPageButton_Clicked(object sender, EventArgs e)
        {
            Title = "Настройки";
            MainFrame.Content = SettingsContentView;
        }

        public void DA(string title, string message, string btn)
        {
            DisplayAlert(title, message, btn);
        }

    }

}
