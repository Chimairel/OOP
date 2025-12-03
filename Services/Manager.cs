using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Services
{
    public class TaskEventArgs : EventArgs
    {
        public TaskItem Task { get; set; }
        public Employee Employee { get; set; }
    }

    public class Manager
    {
        private readonly DataStore _store;
        private DataModel _model;

        public event EventHandler<TaskEventArgs>? TaskAssigned;
        public event EventHandler<TaskEventArgs>? TaskCompleted;

        public Manager(string dataFilePath)
        {
            _store = new DataStore(dataFilePath);
            _model = _store.Load();
        }

        public void Run()
        {
            while (true)
            {
                ShowMenu();
                var opt = Console.ReadLine()?.Trim();
                try
                {
                    switch (opt)
                    {
                        case "1": ListEmployees(); break;
                        case "2": AddEmployee(); break;
                        case "3": ListTasks(); break;
                        case "4": CreateTask(); break;
                        case "5": AssignTask(); break;
                        case "6": CompleteTask(); break;
                        case "7": Search(); break;
                        case "8": Exit(); return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] {ex.Message}");
                }
            }
        }

        private void ShowMenu()
        {
            Console.WriteLine("\n--- Employee Task Manager ---");
            Console.WriteLine("1. List Employees");
            Console.WriteLine("2. Add Employee");
            Console.WriteLine("3. List Tasks");
            Console.WriteLine("4. Create Task");
            Console.WriteLine("5. Assign Task");
            Console.WriteLine("6. Complete Task");
            Console.WriteLine("7. Search (lambda demo)");
            Console.WriteLine("8. Save & Exit");
            Console.Write("Choose: ");
        }

        #region Employees & Tasks operations
        private void ListEmployees()
        {
            if (!_model.Employees.Any())
            {
                Console.WriteLine("No employees.");
                return;
            }

            foreach (var e in _model.Employees)
                e.PrintInfo();
        }

        private void AddEmployee()
        {
            Console.Write("First name: ");
            var fn = Console.ReadLine() ?? "";
            Console.Write("Last name: ");
            var ln = Console.ReadLine() ?? "";
            Console.Write("Position: ");
            var pos = Console.ReadLine() ?? "";

            var emp = new Employee(_model.NextEmployeeId++, fn, ln, pos);
            _model.Employees.Add(emp);
            _store.Save(_model);
            Console.WriteLine($"Added employee {emp.FullName} (ID {emp.Id})");
        }

        private void ListTasks()
        {
            if (!_model.Tasks.Any())
            {
                Console.WriteLine("No tasks.");
                return;
            }

            foreach (var t in _model.Tasks.OrderBy(t => t.Id))
                Console.WriteLine(t.ToString());
        }

        private void CreateTask()
        {
            Console.Write("Task title: ");
            var title = Console.ReadLine() ?? "";
            Console.Write("Task description: ");
            var desc = Console.ReadLine() ?? "";

            var task = new TaskItem { Id = _model.NextTaskId++, Title = title, Description = desc };
            _model.Tasks.Add(task);
            _store.Save(_model);
            Console.WriteLine($"Created task #{task.Id}");
        }

        private void AssignTask()
        {
            Console.Write("Task ID to assign: ");
            if (!int.TryParse(Console.ReadLine(), out int tid))
                throw new Exception("Invalid Task ID");

            var task = _model.Tasks.FirstOrDefault(t => t.Id == tid) ?? throw new Exception("Task not found");

            Console.Write("Employee ID: ");
            if (!int.TryParse(Console.ReadLine(), out int eid))
                throw new Exception("Invalid Employee ID");

            var emp = _model.Employees.FirstOrDefault(e => e.Id == eid) ?? throw new Exception("Employee not found");

            task.AssignedEmployeeId = emp.Id;
            emp.AssignTask(task.Id);

            _store.Save(_model);

            TaskAssigned?.Invoke(this, new TaskEventArgs { Task = task, Employee = emp });
            Console.WriteLine($"Assigned task #{task.Id} to {emp.FullName}");
        }

        private void CompleteTask()
        {
            Console.Write("Task ID to complete: ");
            if (!int.TryParse(Console.ReadLine(), out int tid))
                throw new Exception("Invalid Task ID");

            var task = _model.Tasks.FirstOrDefault(t => t.Id == tid) ?? throw new Exception("Task not found");
            if (task.IsCompleted)
            {
                Console.WriteLine("Task already completed.");
                return;
            }

            var emp = _model.Employees.FirstOrDefault(e => e.Id == task.AssignedEmployeeId);
            task.IsCompleted = true;
            if (emp != null)
                emp.CompleteTask(task.Id);

            _store.Save(_model);

            TaskCompleted?.Invoke(this, new TaskEventArgs { Task = task, Employee = emp });
            Console.WriteLine($"Task #{task.Id} marked completed.");
        }
        #endregion

        #region Search / Lambda expression demo
        private void Search()
        {
            Console.WriteLine("Search options:");
            Console.WriteLine("1. Employees by name contains");
            Console.WriteLine("2. Tasks by title contains");
            Console.Write("Choose: ");
            var opt = Console.ReadLine();

            switch (opt)
            {
                case "1":
                    Console.Write("Enter name fragment: ");
                    var frag = Console.ReadLine() ?? "";
                    var results = _model.Employees
                        .Where(e => e.FullName.Contains(frag, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    Console.WriteLine($"Found {results.Count} employees:");
                    results.ForEach(e => Console.WriteLine($" - {e.Id}: {e.FullName}"));
                    break;

                case "2":
                    Console.Write("Enter task title fragment: ");
                    var tfrag = Console.ReadLine() ?? "";
                    var tres = _model.Tasks
                        .Where(t => t.Title.Contains(tfrag, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    Console.WriteLine($"Found {tres.Count} tasks:");
                    tres.ForEach(t => Console.WriteLine($" - {t.Id}: {t.Title} (Assigned: {t.AssignedEmployeeId})"));
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        #endregion

        private void Exit()
        {
            _store.Save(_model);
            Console.WriteLine("Data saved. Goodbye!");
        }
    }
}
