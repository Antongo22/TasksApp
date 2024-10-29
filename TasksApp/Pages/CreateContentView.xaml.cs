using System.ComponentModel;
using TasksApp.Data;
namespace TasksApp.Pages;

public class TaskRepetitionItem
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
}

public class CreateContentViewModel : INotifyPropertyChanged
{
    public List<TaskRepetitionItem> Repetitions { get; set; }

    private TaskRepetitionItem _selectedRepetition;
    public TaskRepetitionItem SelectedRepetition
    {
        get => _selectedRepetition;
        set
        {
            _selectedRepetition = value;
            OnPropertyChanged(nameof(SelectedRepetition));
        }
    }

    public CreateContentViewModel()
    {
        Repetitions = new List<TaskRepetitionItem>
        {
            new TaskRepetitionItem { Id = "never", DisplayName = "Никогда" },
            new TaskRepetitionItem { Id = "every_day", DisplayName = "Каждый день" },
            new TaskRepetitionItem { Id = "every_week", DisplayName = "Каждую неделю" },
            new TaskRepetitionItem { Id = "every_month", DisplayName = "Каждый месяц" },
            new TaskRepetitionItem { Id = "every_year", DisplayName = "Каждый год" },
        };

        // Устанавливаем "Никогда" как выбранное значение по умолчанию
        SelectedRepetition = Repetitions.FirstOrDefault(r => r.Id == "never");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}




public partial class CreateContentView : ContentView
{
    private TaskDatabase _taskDatabase; // Экземпляр базы данных
    MainPage ParentContentPage;

    public CreateContentView(MainPage ParentContentPage)
    {
        InitializeComponent();
        BindingContext = new CreateContentViewModel();

        this.ParentContentPage = ParentContentPage;
        this._taskDatabase = TaskDatabase.GetInstance();

        TaskDatePicker.MinimumDate = DateTime.Today;
    }

    /// <summary>
    /// Обработчик для переключения даты
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDateSwitchToggled(object sender, ToggledEventArgs e)
    {
        TaskDatePicker.IsEnabled = e.Value;

        if (e.Value)
        {
            TaskDatePicker.MinimumDate = DateTime.Today;
        }
        else
        {
            TimeSwitch.IsToggled = false;
            TaskTimePicker.IsEnabled = false;
        }
    }


    /// <summary>
    /// Обработчик для переключения времени
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (!TaskDatePicker.IsEnabled)
        {
            TimeSwitch.IsToggled = false;
            return;
        }

        TaskTimePicker.IsEnabled = e.Value; 

        if (e.Value && TaskDatePicker.Date == DateTime.Today)
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5));
            TaskTimePicker.Time = currentTime; 
            TaskTimePicker.PropertyChanged += TaskTimePicker_PropertyChanged;
        }
        else
        {
            TaskTimePicker.PropertyChanged -= TaskTimePicker_PropertyChanged;
        }
    }



    private bool isTimeBeingUpdated = false;  // Флаг для предотвращения рекурсии
    private void TaskTimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (isTimeBeingUpdated)
        {
            isTimeBeingUpdated = false;
            return; 
        }

        if (e.PropertyName == nameof(TimePicker.Time))
        {
            if (TaskDatePicker.Date == DateTime.Today && TaskTimePicker.Time < DateTime.Now.TimeOfDay)
            {
                ShowAlert("Ошибка", "Нельзя выбрать время, которое уже прошло", "OK");
                TaskTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5));

                isTimeBeingUpdated = true;
            }
        }
    }

    /// <summary>
    /// Обработчик нажатия кнопки для создания задачи
    /// </summary>
    private void OnCreateTaskButtonClicked(object sender, EventArgs e)
    {
        // Получаем ViewModel из BindingContext
        var viewModel = BindingContext as CreateContentViewModel;

        string taskTitle = TaskTitleEntry.Text;
        string taskDescription = TaskDescriptionEditor.Text;
        DateTime? taskDate = TaskDatePicker.IsEnabled ? TaskDatePicker.Date : (DateTime?)null;
        TimeSpan? taskTime = TaskTimePicker.IsEnabled ? TaskTimePicker.Time : (TimeSpan?)null;
        string taskStatus = "process";

        string taskRepeatId = (viewModel.SelectedRepetition != null) ? viewModel.SelectedRepetition.Id : "never";

        if (string.IsNullOrWhiteSpace(taskTitle))
        {
            ShowAlert("Ошибка", "Пожалуйста, введите название задачи.", "OK");
            return;
        }

        DateTime? combinedDateTime = null;

        if (taskDate.HasValue && taskTime.HasValue)
        {
            combinedDateTime = taskDate.Value.Date.Add(taskTime.Value); 

            if (combinedDateTime < DateTime.Now)
            {
                ShowAlert("Ошибка", "Дата и время не могут быть в прошлом.", "OK");
                return;
            }
        }

        var newTask = new TaskModel
        {
            Name_Tasks = taskTitle,
            Description_Tasks = taskDescription,
            Date_Of_End_Tasks = combinedDateTime,
            Repetitions_Tasks = taskRepeatId, 
            Status_Tasks = taskStatus
        };

        _taskDatabase.AddTask(newTask);

        ShowAlert("Задача создана", $"Задача \"{taskTitle}\" создана.\nДата - {combinedDateTime}", "OK");

        ParentContentPage.CreateContentView = new(ParentContentPage);
        ParentContentPage.SetMainContentView();
        ParentContentPage.MainContentView.LoadTasks();
    }





    private void ShowAlert(string title, string message, string btn)
    {
        ParentContentPage.DA(title, message, btn);
    }
}