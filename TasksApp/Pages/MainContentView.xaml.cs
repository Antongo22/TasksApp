using TasksApp.Data;
using Microsoft.Maui.Controls;

public static class TaskRepetitionHelper
{
    public static string ToDisplayString(string repetitionId)
    {
        return repetitionId switch
        {
            "never" => "Никогда",
            "every_day" => "Каждый день",
            "every_week" => "Каждую неделю",
            "every_month" => "Каждый месяц",
            "every_year" => "Каждый год",
            _ => "Неизвестно" // Значение по умолчанию
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
        /// Метод для загрузки задач из базы данных и отображения их блоками.
        /// </summary>
        public void LoadTasks()
        {
            // Очищаем контейнер перед загрузкой новых данных
            TasksContainer.Children.Clear();

            var tasks = _taskDatabase.GetTasks(); // Получаем список задач

            foreach (var task in tasks)
            {
                // Создаем блок для каждой задачи
                var taskBlock = CreateTaskBlock(task);

                // Добавляем блок в контейнер
                TasksContainer.Children.Add(taskBlock);
            }
        }

        /// <summary>
        /// Метод для создания визуального блока для задачи
        /// </summary>
        /// <param name="task">Модель задачи</param>
        /// <returns>Возвращает Frame с данными задачи</returns>
        private Frame CreateTaskBlock(TaskModel task)
        {
            // Создаем лейблы для отображения данных задачи
            var titleLabel = new Label
            {
                Text = task.Name_Tasks,
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.White // Цвет текста
            };

            var descriptionLabel = new Label
            {
                Text = string.IsNullOrEmpty(task.Description_Tasks) ? "Без описания" : task.Description_Tasks,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.White, // Цвет текста
                LineBreakMode = LineBreakMode.TailTruncation, // Обрезаем текст с многоточием в конце
                MaxLines = 2 // Максимальное количество строк
            };

            // Проверка, чтобы не выводить время, если оно равно 00:00
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
                dateTimeText = "Без даты";
            }

            var dateLabel = new Label
            {
                Text = dateTimeText,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.Gray // Цвет текста
            };

            var repetitionLabel = new Label
            {
                Text = $"Повторение: {TaskRepetitionHelper.ToDisplayString(task.Repetitions_Tasks)}",
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.Gray // Цвет текста
            };

            var statusLabel = new Label
            {
                Text = $"Статус: {task.Status_Tasks}",
                FontSize = 12,
                FontAttributes = FontAttributes.Italic,
                TextColor = Color.FromArgb("#ccc") // Цвет текста
            };

            // Создаем StackLayout для объединения элементов задачи
            var taskLayout = new VerticalStackLayout
            {
                Spacing = 5,
                Children = { titleLabel, descriptionLabel, dateLabel, repetitionLabel, statusLabel }
            };

            // Создаем карточку (Frame) для задачи с фиксированной высотой
            var taskFrame = new Frame
            {
                Content = taskLayout,
                BorderColor = Color.FromArgb("#444"),
                CornerRadius = 8,
                Padding = 10,
                BackgroundColor = Color.FromArgb("#222"), // Темный фон
                HeightRequest = 150 // Задаем фиксированную высоту блока задачи
            };

            return taskFrame;
        }

    }
}
