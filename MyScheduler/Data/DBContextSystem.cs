using MyGP2webapp.Models;
using Microsoft.EntityFrameworkCore;


namespace MyGP2webapp.Data
{
    public class DBContextSystem : DbContext
    {
        public DBContextSystem(DbContextOptions<DBContextSystem> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<DegreeProgressPlan> degreeProgressPlans { get; set; }
        public DbSet<DegreeProgresContent> degree_Contents { get; set; }
        public DbSet<StudyPlan> studyPlan { get; set; }
        public DbSet<PlanContent> plan_Content { get; set; }
        public DbSet<Schedule> schedules { get; set; }
        public DbSet<Section> sections { get; set; }
        public DbSet<SectionSchedule> sectionSchedules { get; set; }
        public DbSet<Progress> progresses { get; set; }
        public DbSet<Instructors> instractors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }


}
