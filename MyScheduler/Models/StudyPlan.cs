using System.ComponentModel.DataAnnotations;

namespace MyGP2webapp.Models
{
    public class StudyPlan
    {
        [Key]
        public int IdStudyPlan { get; set; }
        [Required]
        public string Name { get; set; }
        public string College { get; set; }
        public string Major {  get; set; }
    }
}
