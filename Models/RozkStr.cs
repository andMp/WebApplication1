
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
        public string Opis { get; set; }

        [Required]
        public int StreamerId { get; set; }

        [Required]
        public DateTime Pochatok { get; set; }
        public StrStatus Status { get; set; } = StrStatus.Ochikuytsa;
        public DateTime ChasStvor { get; set; } = DateTime.UtcNow;
        public int ReminderMinutes { get; set; } = 0;
    }

    public class CreateStreamDto
    {
        [Required, MaxLength(200)]
        public string Nazva { get; set; } = default!;
        public string? Opis { get; set; }
        [Required]
        public int StreamerId { get; set; }
        [Required]
        public DateTime Pochatok { get; set; }
        public StrStatus Status { get; set; } = StrStatus.Ochikuytsa;
        public DateTime ChasStvor { get; set; } = DateTime.UtcNow;
        public int? ReminderMinutes { get; set; }
    }

    public class UpdateStatusDto
    {
        [Required]
        public StrStatus Status { get; set; }
    }
}

