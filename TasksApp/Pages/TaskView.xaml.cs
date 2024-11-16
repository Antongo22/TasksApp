using TasksApp.Data;
using Microsoft.Maui.Controls;

namespace TasksApp.Pages
{
    public partial class TaskView : ContentView
    {
        TaskModel task;
        TaskDatabase _taskDatabase = TaskDatabase.GetInstance();
        MainPage mainPage;

        public TaskView(TaskModel task, MainPage mainPage)
        {
            this.task = task;
            InitializeComponent();
            BindingContext = task;
            this.mainPage = mainPage;

            DateLabel.Text = FormatDate(task.Date_Of_End_Tasks);
            RepetitionLabel.Text = $"����������: {TaskRepetitionHelper.ToDisplayString(task.Repetitions_Tasks)}";
        }

        private void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            task.IsCompleted = e.Value;
            _taskDatabase.UpdateTask(task);
        }

        private string FormatDate(DateTime? date)
        {
            if (date.HasValue)
            {
                var dateTime = date.Value;
                return dateTime.TimeOfDay.TotalHours == 0
                    ? dateTime.ToString("dd.MM.yyyy")
                    : dateTime.ToString("dd.MM.yyyy HH:mm");
            }
            return "��� ����";
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            // ���������� ���������� ���� �������������
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "������������� ��������",
                "�� �������, ��� ������ ������� ��� ������?",
                "��", "������");

            if (confirm && task != null)
            {
                // ������� ������ � ������������ � ������ �����
                _taskDatabase.DeleteTask(task.ID_Tasks);
                mainPage.SetMainContentView();
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            // ��������� �� �������� �������������� ������
            var editContentView = new EditContentView(mainPage, task);
            mainPage.SetMainFrame(editContentView);
        }
    }
}