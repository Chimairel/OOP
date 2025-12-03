using System;
using Services;

namespace OOP2_FinalProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Employee Task Manager - OOP2 Final Project";

            var manager = new Manager("Data/data.json");

            manager.TaskAssigned += (sender, e) =>
            {
                Console.WriteLine($"\n[Event] Task Assigned: '{e.Task.Title}' -> {e.Employee.FullName}\n");
            };

            manager.TaskCompleted += (sender, e) =>
            {
                Console.WriteLine($"\n[Event] Task Completed: '{e.Task.Title}' by {e.Employee.FullName}\n");
            };

            manager.Run();
        }
    }
}
