using System;
using System.ComponentModel;
using TasksApp.Data;

namespace TasksApp.Pages
{
    public class EditContentViewModel : INotifyPropertyChanged
    {
        public TaskModel Task { get; set; }
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

        public EditContentViewModel(TaskModel task)
        {
            Task = task;
            Repetitions = new List<TaskRepetitionItem>
            {
                new TaskRepetitionItem { Id = "never", DisplayName = "�������" },
                new TaskRepetitionItem { Id = "every_day", DisplayName = "������ ����" },
                new TaskRepetitionItem { Id = "every_week", DisplayName = "������ ������" },
                new TaskRepetitionItem { Id = "every_month", DisplayName = "������ �����" },
                new TaskRepetitionItem { Id = "every_year", DisplayName = "������ ���" },
            };

            // ������������� ������� �������� ����������
            SelectedRepetition = Repetitions.FirstOrDefault(r => r.Id == task.Repetitions_Tasks);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class EditContentView : ContentView
    {
        private TaskDatabase _taskDatabase; // ��������� ���� ������
        MainPage ParentContentPage;

        public EditContentView(MainPage parentContentPage, TaskModel task)
        {
            InitializeComponent();
            BindingContext = new EditContentViewModel(task);

            this.ParentContentPage = parentContentPage;
            this._taskDatabase = TaskDatabase.GetInstance();

            TaskDatePicker.MinimumDate = DateTime.Today;

            // ������������� ��������� ��������������
            InitializeSwitches(task);
        }

        private void InitializeSwitches(TaskModel task)
        {
            if (task.Date_Of_End_Tasks.HasValue)
            {
                DateSwitch.IsToggled = true;
                TaskDatePicker.Date = task.Date_Of_End_Tasks.Value;

                if (task.Date_Of_End_Tasks.Value.TimeOfDay != TimeSpan.Zero)
                {
                    TimeSwitch.IsToggled = true;
                    TaskTimePicker.Time = task.Date_Of_End_Tasks.Value.TimeOfDay;
                }
                else
                {
                    TimeSwitch.IsToggled = false;
                    TaskTimePicker.IsEnabled = false;
                }
            }
            else
            {
                DateSwitch.IsToggled = false;
                TimeSwitch.IsToggled = false;
                TaskDatePicker.IsEnabled = false;
                TaskTimePicker.IsEnabled = false;
            }
        }

        /// <summary>
        /// ���������� ��� ������������ ����
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
        /// ���������� ��� ������������ �������
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

        private bool isTimeBeingUpdated = false;  // ���� ��� �������������� ��������
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
                    ShowAlert("������", "������ ������� �����, ������� ��� ������", "OK");
                    TaskTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5));

                    isTimeBeingUpdated = true;
                }
            }
        }

        /// <summary>
        /// ���������� ������� ������ ��� ���������� ���������
        /// </summary>
        private void OnSaveTaskButtonClicked(object sender, EventArgs e)
        {
            // �������� ViewModel �� BindingContext
            var viewModel = BindingContext as EditContentViewModel;

            string taskTitle = TaskTitleEntry.Text;
            string taskDescription = TaskDescriptionEditor.Text;
            DateTime? taskDate = TaskDatePicker.IsEnabled ? TaskDatePicker.Date : (DateTime?)null;
            TimeSpan? taskTime = TaskTimePicker.IsEnabled ? TaskTimePicker.Time : (TimeSpan?)null;

            string taskRepeatId = (viewModel.SelectedRepetition != null) ? viewModel.SelectedRepetition.Id : "never";

            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                ShowAlert("������", "����������, ������� �������� ������.", "OK");
                return;
            }

            DateTime? combinedDateTime = null;
            DateTime? checkcombinedDateTime = null;
            if (taskDate.HasValue)
            {
                if (taskTime.HasValue)
                {
                    combinedDateTime = taskDate.Value.Date.Add(taskTime.Value);
                    checkcombinedDateTime = taskDate.Value.Date.Add(taskTime.Value);
                }
                else
                {
                    combinedDateTime = taskDate.Value.Date.Add(TimeSpan.Zero); 
                    checkcombinedDateTime = taskDate.Value.Date.Add(new TimeSpan(23, 59, 0));
                }

                if (checkcombinedDateTime < DateTime.Now)
                {
                    ShowAlert("������", "���� � ����� �� ����� ���� � �������.", "OK");
                    return;
                }
            }

            var updatedTask = new TaskModel
            {
                ID_Tasks = viewModel.Task.ID_Tasks,
                Name_Tasks = taskTitle,
                Description_Tasks = taskDescription,
                Date_Of_End_Tasks = combinedDateTime,
                Repetitions_Tasks = taskRepeatId,
                Status_Tasks = viewModel.Task.Status_Tasks
            };

            _taskDatabase.UpdateTask(updatedTask);

            ShowAlert("������ ���������", $"������ \"{taskTitle}\" ���������.\n���� - {combinedDateTime}", "OK");

            ParentContentPage.SetMainContentView();
            ParentContentPage.MainContentView.LoadTasks();
        }

        private void ShowAlert(string title, string message, string btn)
        {
            ParentContentPage.DA(title, message, btn);
        }
    }
}