using TasksApp.Pages;
using TasksApp.Data;

namespace TasksApp;

public partial class MainPage : ContentPage
{
    public MainContentView MainContentView;
    public CreateContentView CreateContentView;
    public SettingsContentView SettingsContentView;
    ContentView SelectedContentView;
    ContentView LastContentView;

    public MainPage()
    {
        TaskDatabase.GetInstance().UpdateOverdueTasks();
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        MainContentView = new(this);
        CreateContentView = new(this);
        SettingsContentView = new();
        SetMainContentView();
    }

    private void MainPageButton_Clicked(object sender, EventArgs e)
    {
        Title = "Задачи";
        SetMainContentView();
    }

    public void SetMainContentView()
    {
        SetMainFrame(MainContentView);
        MainContentView.LoadTasks();
    }

    private void CreatePageButton_Clicked(object sender, EventArgs e)
    {
        Title = "Создать задачу";
        SetMainFrame(CreateContentView);
    }

    private void SettingsPageButton_Clicked(object sender, EventArgs e)
    {
        Title = "Настройки";
        SetMainFrame(SettingsContentView);
    }

    public void DA(string title, string message, string btn)
    {
        DisplayAlert(title, message, btn);
    }

    public void SetMainFrame(ContentView contentView)
    {
        LastContentView = SelectedContentView;
        SelectedContentView = contentView;
        MainFrame.Content = contentView;
    }

    protected override bool OnBackButtonPressed()
    {
        if (MainFrame.Content == MainContentView)
        {
            ConfirmExit();
            return true; 
        }
        else if (SelectedContentView is EditContentView)
        {
            SetMainFrame(LastContentView);
            return true;
        }
        else
        {
            SetMainContentView();
            Title = "Задачи";
            return true; 
        }
    }

    private async void ConfirmExit()
    {
        bool answer = await DisplayAlert("Подтверждение", "Точно ли вы хотите выйти?", "Да", "Нет");
        if (answer)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }
    }

}
