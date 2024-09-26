using System.ComponentModel.DataAnnotations;

namespace MyGP2webapp.Models
{
    public class Progress
    {
        [Key]
        [Required]
        public int IdProgress { get; set; }
        public float Mark {  get; set; }

        public Student Student { get; set; }
        public Course course { get; set; }


    }
}
