using System;

namespace Models
{
    public partial class Employee
    {
        public override void PrintInfo()
        {
            Console.WriteLine($"[Employee] ID:{Id} Name:{FullName} Position:{Position} Tasks:{TaskIds.Count}");
        }

        public void AssignTask(int taskId)
        {
            if (!TaskIds.Contains(taskId))
                TaskIds.Add(taskId);
        }

        public void CompleteTask(int taskId)
        {
            if (TaskIds.Contains(taskId))
                TaskIds.Remove(taskId);
        }
    }
}
