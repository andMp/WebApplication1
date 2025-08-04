using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, ErrorMessage = "Назва не може бути довша за 100 символів")]
        public string Title { get; set; }

        [StringLength(300, ErrorMessage = "Опис не може бути довший за 300 символів")]
        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
