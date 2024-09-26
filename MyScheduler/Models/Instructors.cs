using System.ComponentModel.DataAnnotations;

namespace MyGP2webapp.Models
{
    public class Instructors
    {
        [Key]
        public int IdInstructor { get; set; }
        public int Job_ID { get; set; }
        public string Name { get; set; }
        public ICollection<Section> Sections { get; set; }

    }
}
