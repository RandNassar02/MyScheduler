using System.ComponentModel.DataAnnotations;

namespace MyGP2webapp.Models
{
    public class Schedule
    {
        [Key]
        [Required]
        public int IDScedule { get; set; }
        [Required]
        public ICollection<SectionSchedule> sectionSchedules { get; set; }
        public Student students { get; set; }

        public int? Approv_Schedule { get; set; }
    }
}
