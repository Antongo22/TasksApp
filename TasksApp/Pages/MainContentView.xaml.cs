using TasksApp.Data;
using Microsoft.Maui.Controls;

public static class TaskRepetitionHelper
{
    public static string ToDisplayString(string repetitionId)
    {
        return repetitionId switch
        {
            "never" => "�������",
            "every_day" => "������ ����",
            "every_week" => "������ ������",
            "every_month" => "������ �����",
            "every_year" => "������ ���",
            _ => "����������" // �������� �� ���������
        };
    }
}


namespace TasksApp.Pages
{
    public partial class MainContentView : ContentView
    {
        private TaskDatabase _taskDatabase;

        public MainContentView()
        {
            InitializeComponent();
            _taskDatabase = TaskDatabase.GetInstance();
            LoadTasks();
        }

        /// <summary>
        /// ����� ��� �������� ����� �� ���� ������ � ����������� �� �������.
        /// </summary>
        public void LoadTasks()
        {
            // ������� ��������� ����� ��������� ����� ������
            TasksContainer.Children.Clear();

            var tasks = _taskDatabase.GetTasks(); // �������� ������ �����

            foreach (var task in tasks)
            {
                // ������� ���� ��� ������ ������
                var taskBlock = CreateTaskBlock(task);

                // ��������� ���� � ���������
                TasksContainer.Children.Add(taskBlock);
            }
        }

        /// <summary>
        /// ����� ��� �������� ����������� ����� ��� ������
        /// </summary>
        /// <param name="task">������ ������</param>
        /// <returns>���������� Frame � ������� ������</returns>
        private Frame CreateTaskBlock(TaskModel task)
        {
            // ������� ������ ��� ����������� ������ ������
            var titleLabel = new Label
            {
                Text = task.Name_Tasks,
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.White // ���� ������
            };

            var descriptionLabel = new Label
            {
                Text = string.IsNullOrEmpty(task.Description_Tasks) ? "��� ��������" : task.Description_Tasks,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.White, // ���� ������
                LineBreakMode = LineBreakMode.TailTruncation, // �������� ����� � ����������� � �����
                MaxLines = 2 // ������������ ���������� �����
            };

            // ��������, ����� �� �������� �����, ���� ��� ����� 00:00
            string dateTimeText;
            if (task.Date_Of_End_Tasks.HasValue)
            {
                var dateTime = task.Date_Of_End_Tasks.Value;
                dateTimeText = dateTime.TimeOfDay.TotalHours == 0 ?
                    dateTime.ToString("dd.MM.yyyy") :
                    dateTime.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                dateTimeText = "��� ����";
            }

            var dateLabel = new Label
            {
                Text = dateTimeText,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.Gray // ���� ������
            };

            var repetitionLabel = new Label
            {
                Text = $"����������: {TaskRepetitionHelper.ToDisplayString(task.Repetitions_Tasks)}",
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.Gray // ���� ������
            };

            var statusLabel = new Label
            {
                Text = $"������: {task.Status_Tasks}",
                FontSize = 12,
                FontAttributes = FontAttributes.Italic,
                TextColor = Color.FromArgb("#ccc") // ���� ������
            };

            // ������� StackLayout ��� ����������� ��������� ������
            var taskLayout = new VerticalStackLayout
            {
                Spacing = 5,
                Children = { titleLabel, descriptionLabel, dateLabel, repetitionLabel, statusLabel }
            };

            // ������� �������� (Frame) ��� ������ � ������������� �������
            var taskFrame = new Frame
            {
                Content = taskLayout,
                BorderColor = Color.FromArgb("#444"),
                CornerRadius = 8,
                Padding = 10,
                BackgroundColor = Color.FromArgb("#222"), // ������ ���
                HeightRequest = 150 // ������ ������������� ������ ����� ������
            };

            return taskFrame;
        }

    }
}
