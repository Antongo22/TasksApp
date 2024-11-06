using TasksApp.Data;
using Microsoft.Maui.Controls;

namespace TasksApp.Pages
{
    public partial class TaskView : ContentView
    {
        TaskModel task;
        TaskDatabase _taskDatabase = TaskDatabase.GetInstance();

        public TaskView(TaskModel task)
        {
            this.task = task;
            InitializeComponent();
            BindingContext = task;

            // ������������� ����������������� ��������
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
    }
}
