//using System.ComponentModel.DataAnnotations;

//namespace WebApplication1.Models
//{
//    public enum StrStatus { Ochikuytsa = 0, Zaverseni = 1 }
//    public class RozkStr
//    {

//        [Required(ErrorMessage = "Назва обов'язкова")]
//        [StringLength(100, ErrorMessage = "Назва не може бути довша за 100 символів")]
//        public string Title { get; set; }

//        [StringLength(300, ErrorMessage = "Опис не може бути довший за 300 символів")]
//        public string Description { get; set; }

//        public bool IsCompleted { get; set; }
//    }
//}

using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public enum StrStatus { Ochikuytsa = 0, Zaverseni = 1 }

    public class RozkStr
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Nazva { get; set; } = default!;

        [Required]
        public int StreamerId { get; set; }

        [Required]
        public DateTime Pochatok { get; set; }
        public StrStatus Status { get; set; } = StrStatus.Ochikuytsa;
        public DateTime ChasStvor { get; set; } = DateTime.UtcNow;
        public bool ReminderSent { get; set; } = false;
    }

    public class CreateStreamDto
    {
        [Required, MaxLength(200)]
        public string Nazva { get; set; } = default!;
        [Required]
        public int StreamerId { get; set; }
        [Required]
        public DateTime Pochatok { get; set; }
    }

    public class UpdateStatusDto
    {
        [Required]
        public StrStatus Status { get; set; }
    }
}

