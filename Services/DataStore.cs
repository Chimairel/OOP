using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Models;

namespace Services
{
    public class DataModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public int NextEmployeeId { get; set; } = 1;
        public int NextTaskId { get; set; } = 1;
    }

    public class DataStore
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { WriteIndented = true };

        public DataStore(string filePath)
        {
            _filePath = filePath;
            EnsureDirectory();
        }

        private void EnsureDirectory()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public DataModel Load()
        {
            try
            {
                // If file doesn't exist, create empty model and save
                if (!File.Exists(_filePath))
                {
                    var dm = new DataModel();
                    Save(dm);
                    return dm;
                }

                var json = File.ReadAllText(_filePath);

                // Handle empty file
                if (string.IsNullOrWhiteSpace(json))
                {
                    var dm = new DataModel();
                    Save(dm);
                    return dm;
                }

                // Try to deserialize
                var model = JsonSerializer.Deserialize<DataModel>(json);
                if (model == null)
                {
                    // In case JSON is invalid but file not empty
                    var dm = new DataModel();
                    Save(dm);
                    return dm;
                }

                return model;
            }
            catch (Exception ex)
            {
                // Handle invalid JSON gracefully
                Console.WriteLine($"Error loading data: {ex.Message}");
                var dm = new DataModel();
                Save(dm);
                return dm;
            }
        }

        public void Save(DataModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model, _options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }
    }
}
