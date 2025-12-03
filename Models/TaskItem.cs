using System;

namespace Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AssignedEmployeeId { get; set; } = -1;
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            var assigned = AssignedEmployeeId >= 0 ? AssignedEmployeeId.ToString() : "Unassigned";
            return $"[Task #{Id}] {Title} (Assigned: {assigned}) Completed:{IsCompleted}";
        }
    }
}
