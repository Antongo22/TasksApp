﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TasksApp.Data
{
    public class TaskModel
    {
        public int ID_Tasks { get; set; }
        public string Name_Tasks { get; set; }
        public string? Description_Tasks { get; set; }
        public DateTime? Date_Of_End_Tasks { get; set; }
        public string Repetitions_Tasks { get; set; }
        public string Status_Tasks { get; set; }
    }
}
