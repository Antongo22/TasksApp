using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace TasksApp.Data
{
    public class TaskDatabase
    {
        private static TaskDatabase _instance;
        private static readonly object _lock = new object();


        private readonly string _connectionString;

        private TaskDatabase()
        {
            string folderPath = FileSystem.AppDataDirectory;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string dbPath = Path.Combine(folderPath, "tasks.db");

            _connectionString = $"Data Source={Path.Combine(FileSystem.AppDataDirectory, "tasks.db")}";
            CreateTable();
        }


        /// <summary>
        /// Публичное свойство для доступа к единственному экземпляру класса (Singleton)
        /// </summary>
        public static TaskDatabase GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock) 
                {
                    if (_instance == null)
                    {
                        _instance = new TaskDatabase();
                    }
                }
            }
            return _instance;
        }



        /// <summary>
        /// Метод для создания таблицы
        /// </summary>
        private void CreateTable()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        ID_Tasks INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name_Tasks TEXT NOT NULL,
                        Description_Tasks TEXT,
                        Date_Of_End_Tasks DATETIME,
                        Repetitions_Tasks TEXT NOT NULL,
                        Status_Tasks TEXT NOT NULL
                    );
            ";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Метод для добавления задачи
        /// </summary>
        /// <param name="task">Класс, представляющий задачу</param>
        public void AddTask(TaskModel task)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
            INSERT INTO Tasks (Name_Tasks, Description_Tasks, Date_Of_End_Tasks, Repetitions_Tasks, Status_Tasks)
            VALUES ($name, $description, $dateOfEnd, $repetitions, $status);
            ";
                command.Parameters.AddWithValue("$name", task.Name_Tasks);
                command.Parameters.AddWithValue("$description", task.Description_Tasks ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("$dateOfEnd", task.Date_Of_End_Tasks?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("$repetitions", task.Repetitions_Tasks);
                command.Parameters.AddWithValue("$status", task.Status_Tasks);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Метод для получения всех задач
        /// </summary>
        /// <returns></returns>
        public List<TaskModel> GetTasks()
        {
            var tasks = new List<TaskModel>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Tasks;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var task = new TaskModel
                        {
                            ID_Tasks = reader.GetInt32(0),
                            Name_Tasks = reader.GetString(1),
                            Description_Tasks = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Date_Of_End_Tasks = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            Repetitions_Tasks = reader.GetString(4),
                            Status_Tasks = reader.GetString(5)
                        };
                        tasks.Add(task);
                    }
                }
            }
            return tasks;
        }

        /// <summary>
        /// Метод для обновления задачи
        /// </summary>
        /// <param name="task">Класс, представляющий задачу</param>
        public void UpdateTask(TaskModel task)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
            UPDATE Tasks
            SET Name_Tasks = $name, 
                Description_Tasks = $description, 
                Date_Of_End_Tasks = $dateOfEnd,
                Repetitions_Tasks = $repetitions,
                Status_Tasks = $status
            WHERE ID_Tasks = $id;
            ";
                command.Parameters.AddWithValue("$name", task.Name_Tasks);
                command.Parameters.AddWithValue("$description", task.Description_Tasks ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("$dateOfEnd", task.Date_Of_End_Tasks?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("$repetitions", task.Repetitions_Tasks);
                command.Parameters.AddWithValue("$status", task.Status_Tasks);
                command.Parameters.AddWithValue("$id", task.ID_Tasks);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Метод для удаления задачи
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        public void DeleteTask(int taskId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Tasks WHERE ID_Tasks = $id;";
                command.Parameters.AddWithValue("$id", taskId);
                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Метод для обновления даты задач, если они просрочены и имеют повторение
        /// </summary>
        public void UpdateOverdueTasks()
        {
            var tasks = GetTasks();
            foreach (var task in tasks)
            {
                if (task.Date_Of_End_Tasks.HasValue && task.Date_Of_End_Tasks.Value < DateTime.Now && task.Repetitions_Tasks != "never")
                {
                    DateTime newDate = task.Date_Of_End_Tasks.Value;

                    switch (task.Repetitions_Tasks)
                    {
                        case "every day":
                            newDate = newDate.AddDays(1);
                            break;
                        case "every week":
                            newDate = newDate.AddDays(7);
                            break;
                        case "every month":
                            newDate = newDate.AddMonths(1);
                            break;
                        case "every year":
                            newDate = newDate.AddYears(1);
                            break;
                        default:
                            // Если значение не соответствует ни одному из известных, можно либо пропустить, либо выбросить исключение
                            continue;
                    }

                    task.Date_Of_End_Tasks = newDate;
                    UpdateTask(task);
                }
            }
        }
    }
}
