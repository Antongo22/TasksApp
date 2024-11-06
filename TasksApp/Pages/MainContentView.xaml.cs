using TasksApp.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

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
        MainPage _mainPage;

        public MainContentView(MainPage mainPage)
        {
            InitializeComponent();
            _taskDatabase = TaskDatabase.GetInstance();
            _mainPage = mainPage;
        }

        /// <summary>
        /// Метод для загрузки задач из базы данных и отображения их блоками.
        /// </summary>
        public void LoadTasks()
        {
            TasksContainer.Children.Clear();

            var tasks = _taskDatabase.GetTasks();

            var sortedTasks = tasks.OrderByDescending(task => !task.IsCompleted)
                                   .ThenBy(task => task.Date_Of_End_Tasks)
                                   .ToList();

            foreach (var task in sortedTasks)
            {
                var taskBlock = CreateTaskBlock(task);

                TasksContainer.Children.Add(taskBlock);
            }
        }


        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
        }

        private string TruncateMultilineText(string text, int maxLines, int maxCharsPerLine)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > maxCharsPerLine)
                {
                    lines.Add(currentLine);
                    currentLine = word;

                    if (lines.Count == maxLines - 1) 
                    {
                        currentLine = word;
                        break;
                    }
                }
                else
                {
                    currentLine += (currentLine.Length > 0 ? " " : "") + word;
                }
            }

            if (lines.Count == maxLines - 1)
            {
                currentLine += "...";
                lines.Add(currentLine);
            }
            else if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return string.Join("\n", lines);
        }



        /// <summary>
        /// Метод для создания визуального блока для задачи
        /// </summary>
        /// <param name="task">Модель задачи</param>
        /// <returns>Возвращает Frame с данными задачи</returns>
        private Frame CreateTaskBlock(TaskModel task)
        {
            var titleLabel = new Label
            {
                Text = TruncateText(task.Name_Tasks, 20),
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.White,
                LineBreakMode = LineBreakMode.NoWrap, 
                MaxLines = 1
            };

            var descriptionLabel = new Label
            {
                Text = TruncateMultilineText(string.IsNullOrEmpty(task.Description_Tasks) ? "Без описания" : task.Description_Tasks, 3, 30),
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.White,
                LineBreakMode = LineBreakMode.WordWrap, 
                MaxLines = 3
            };


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
                TextColor = Colors.Gray
            };

            var repetitionLabel = new Label
            {
                Text = $"Повторение: {TaskRepetitionHelper.ToDisplayString(task.Repetitions_Tasks)}",
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.Gray
            };

            var checkBox = new CheckBox
            {
                IsChecked = task.IsCompleted,
                WidthRequest = 30,
                HeightRequest = 30,
                BackgroundColor = Color.FromArgb("#222"),
                Color = Color.FromArgb("#9880e5")
            };

            checkBox.CheckedChanged += (sender, e) =>
            {
                if (sender is CheckBox cb)
                {
                    task.IsCompleted = cb.IsChecked;
                    _taskDatabase.UpdateTask(task);
                    LoadTasks();
                }
            };

            var taskLayout = new VerticalStackLayout
            {
                Spacing = 5,
                Children = { titleLabel, descriptionLabel, dateLabel, repetitionLabel }
            };

            var absoluteLayout = new AbsoluteLayout
            {
                Children = { taskLayout }
            };

            AbsoluteLayout.SetLayoutFlags(checkBox, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(checkBox, new Rect(1, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            absoluteLayout.Children.Add(checkBox);

            var taskFrame = new Frame
            {
                Content = absoluteLayout,
                BorderColor = Color.FromArgb("#444"),
                CornerRadius = 8,
                Padding = 10,
                BackgroundColor = Color.FromArgb("#222"),
                HeightRequest = 125
            };

            taskFrame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    _mainPage.SetMainFrame(new TaskView(task)); 
                })
            });


            return taskFrame;
        }
    }
}