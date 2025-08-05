using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class JsonTaskRepository
    {
        private readonly string FilePath;

        public JsonTaskRepository()
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "data");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Console.WriteLine($"Створено папку: {folder}");
            }

            FilePath = Path.Combine(folder, "tasks.json");
            Console.WriteLine($"Шлях до файлу задач: {FilePath}");
        }

        public List<TaskItem> LoadTasks()
        {
            if (!File.Exists(FilePath))
            {
                // Створення порожнього JSON-файлу, якщо не існує
                File.WriteAllText(FilePath, "[]");
                Console.WriteLine("Файл задач не існував. Створено порожній файл.");
                return new List<TaskItem>();
            }

            try
            {
                var json = File.ReadAllText(FilePath);
                var list = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                Console.WriteLine($"Завантажено {list.Count} задач із файлу.");
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при читанні файлу задач: {ex.Message}");
                return new List<TaskItem>();
            }
        }

        public void SaveTasks(List<TaskItem> tasks)
        {
            try
            {
                var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
                Console.WriteLine($"Збережено {tasks.Count} задач у файл: {FilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні задач: {ex.Message}");
            }
        }

        public void Update(TaskItem task)
        {
            var tasks = LoadTasks();
            var existing = tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                SaveTasks(tasks);
            }
        }
    }
}
