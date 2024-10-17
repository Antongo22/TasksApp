namespace TasksApp.Pages;

public partial class CreateContentView : ContentView
{
    MainPage ParentContentPage;

    public CreateContentView(MainPage ParentContentPage)
    {
        InitializeComponent();

        this.ParentContentPage = ParentContentPage;

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

    private void OnCreateTaskButtonClicked(object sender, EventArgs e)
    {
        string taskTitle = TaskTitleEntry.Text;
        string taskDescription = TaskDescriptionEditor.Text;
        DateTime? taskDate = TaskDatePicker.IsEnabled ? TaskDatePicker.Date : (DateTime?)null;
        TimeSpan? taskTime = TaskTimePicker.IsEnabled ? TaskTimePicker.Time : (TimeSpan?)null;
        string taskRepeat = TaskRepeatPicker.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(taskTitle))
        {
            ShowAlert("Ошибка", "Пожалуйста, введите название задачи.", "OK");
            return; 
        }

        // Проверка на устаревшую дату и время
        if (taskDate.HasValue && taskTime.HasValue)
        {
            DateTime combinedDateTime = taskDate.Value.Add(taskTime.Value);

            if (combinedDateTime < DateTime.Now)
            {
                ShowAlert("Ошибка", "Дата и время не могут быть в прошлом.", "OK");
                return; 
            }
        }

        ShowAlert("Задача создана", $"Задача \"{taskTitle}\" создана.", "OK");
    }


    private void ShowAlert(string title, string message, string btn)
    {
        ParentContentPage.DA(title, message, btn);
    }
}