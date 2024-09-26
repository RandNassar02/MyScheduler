using Microsoft.AspNetCore.Mvc;
using MyGP2webapp.Data;
using MyGP2webapp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyGP2webapp.Controllers
{
    public class StudentController : Controller
    {
        private readonly DBContextSystem _context;
        public static Student? student;
        public List<Section> Sections { get; set; }
        public static int totalCreditHours;
        public static int semester;
        public static int year;
        public static bool ready;

        public static List<int> completedCourseIds;
        public static List<Course> coursesFor2 = new List<Course>();
        public static List<Course> studyPlanCourses = new List<Course>();
        public static List<Course> electiveCourses = new List<Course>();
        public static List<Course> majorElectiveCourses = new List<Course>();
        public static int electiveHours = 0;
        public static int majorElectiveHours = 0;
        public static List<List<Section>> possibleSchedules = new List<List<Section>>();


        public StudentController(DBContextSystem context)
        {
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginStuden userLogin)
        {
            if (ModelState.IsValid)
            {
                student = _context.Students.FirstOrDefault(u => u.Email.Equals(userLogin.Email) && u.password.Equals(userLogin.password));

                if (student != null)
                {
                    char firstCher = student.Name.FirstOrDefault();
                    TempData["MasgName"] = $"{firstCher}";
                    TempData.Keep("MasgName");
                    return RedirectToAction("Index", "Home");
                }
                TempData["Msg"] = "Invalid email or password";
            }
            return View();

        }

        //Check if the registration is open 
        public bool IsOpen()
        { return true; }
        public async void CalculateYearAndSemester()
        {
            double totalCompletedCreditHours = _context.progresses
                .Where(p => p.Student.KeyStudent == student.KeyStudent)
                .Join(_context.Courses, p => p.course.IDCRS, c => c.IDCRS, (p, c) => c.CRS_CR_HOURS)
                .Sum();


            int totalCreditHours = 132;
            int semesters = 8;
            double creditHoursPerSemester = totalCreditHours / (double)semesters;

            int currentSemester = (int)Math.Ceiling(totalCompletedCreditHours / creditHoursPerSemester);
            year = (currentSemester + 1) / 2;
            semester = (currentSemester % 2 == 0) ? 2 : 1;

            if (year == 4 && semester == 2)
            {
                float completedTraining = _context.progresses
                    .Where(p => p.Student.KeyStudent == student.KeyStudent && p.course.IDCRS == 51)
                    .Select(p => p.Mark)
                    .FirstOrDefault();

                int hours = FinishedCourses().Count();
                if (hours == 132)
                {
                    if (completedTraining >= 50.0f)
                    {
                        year = 131;
                        semester = 131;

                    }
                    year = 132;
                    semester = 132;

                }
            }
            else if (year == 0)
            {
                year = 1;
                semester = 1;
            }
            else
            {
                if (semester == 2)
                {
                    year++;
                    semester = 1;
                }
                else
                {
                    semester = 2;
                }
            }

            return;
        }
        public List<int> FinishedCourses()
        {
            // Fetch completed course IDs for the student
            var completedCourseIds = _context.progresses
                .Where(p => p.Student.KeyStudent == student.KeyStudent)
                .Select(p => p.course.IDCRS)
                .ToList();

            return completedCourseIds;
        }
        public async Task GetAvailableCoursesAsync()
        {
            coursesFor2.Clear();
            electiveCourses.Clear();
            studyPlanCourses.Clear();
            majorElectiveCourses.Clear();
            possibleSchedules.Clear();

            var completedCourseIds = FinishedCourses();

            var availableCourses = _context.degree_Contents
                .Where(dc => !completedCourseIds.Contains(dc.course.IDCRS))
                .Select(dc => dc.course)
                .Distinct()
                .ToList();

            majorElectiveHours = _context.degree_Contents
                .Where(dc => completedCourseIds.Contains(dc.course.IDCRS))
                .Where(dc => dc.SMST_NO == 5 && dc.SPEC_YYT == 5)
                .Select(dc => dc.course.CRS_CR_HOURS)
                .Distinct()
                .Sum();

            electiveHours = _context.degree_Contents
                .Where(dc => completedCourseIds.Contains(dc.course.IDCRS))
                .Where(dc => dc.SMST_NO == 0 && dc.SPEC_LVL == 0)
                .Select(dc => dc.course.CRS_CR_HOURS)
                .Distinct()
                .Sum();

            // Remove courses that need a prerequisite
            var coursesStudentCanTake = new List<Course>();
            foreach (var course in availableCourses)
            {
                var prerequisiteIds = _context.plan_Content
                    .Where(pc => pc.course.IDCRS == course.IDCRS)
                    .Where(pc => pc.prerequisite != 0)
                    .Select(pc => pc.prerequisite)
                    .ToList();

                if (!prerequisiteIds.Any() || prerequisiteIds.All(prerequisite => completedCourseIds.Contains((int)prerequisite)))
                {
                    coursesStudentCanTake.Add(course);
                }
            }

            var studyPlanId = _context.Students
                .Where(s => s.KeyStudent == student.KeyStudent)
                .Select(s => s.studyPlan.IdStudyPlan)
                .FirstOrDefault();

            // Fetch courses from the student's study plan
            var studyPlanCanTakeCourses = _context.plan_Content
                .Where(pc => pc.StudyPlan.IdStudyPlan == studyPlanId)
                .Where(pc => coursesStudentCanTake.Contains(pc.course))
                .Select(pc => pc.course)
                .Distinct()
                .ToList();

            var addedCourseIds = new HashSet<int>();
            foreach (var course in studyPlanCanTakeCourses)
            {
                var type = _context.degree_Contents
                    .Where(dc => dc.course.IDCRS == course.IDCRS)
                    .Select(dc => new { dc.SMST_NO, dc.SPEC_LVL })
                    .FirstOrDefault();

                if (type != null && !addedCourseIds.Contains(course.IDCRS))
                {
                    if (type.SMST_NO == 0 && type.SPEC_LVL == 0)
                    {
                        electiveCourses.Add(course);
                    }
                    else if (type.SMST_NO == 5 && type.SPEC_LVL == 5)
                    {
                        majorElectiveCourses.Add(course);
                    }
                    else
                    {
                        studyPlanCourses.Add(course);
                    }
                    addedCourseIds.Add(course.IDCRS);
                }
            }
        }
        public async Task MakeScheduler()
        {
            ready = false;
            possibleSchedules = GenerateSchedules(coursesFor2);
            if (possibleSchedules.Count() != 0)
            { ready = true; }
            else
            { ready = false; }
        }

        public List<List<Section>> GenerateSchedules([FromBody] List<Course> courses)
        {
            var sectionsByCourse = new Dictionary<int, List<Section>>();

            // Retrieve all sections for each course
            foreach (var course in courses)
            {
                var sections = _context.sections.Include(s => s.Instructors).Include(s => s.course)
                                       .Where(s => s.course.IDCRS == course.IDCRS && s.Status == "open")
                                       .ToList();
                sectionsByCourse[course.IDCRS] = sections;
            }

            // Generate all possible schedules
            var allSchedules = GenerateAllSchedules(sectionsByCourse);

            // Return the schedules
            return allSchedules;
        }

        private List<List<Section>> GenerateAllSchedules(Dictionary<int, List<Section>> sectionsByCourse)
        {
            var allSchedules = new List<List<Section>>();

            // Get all possible combinations of sections
            var sectionCombinations = GetCombinations(sectionsByCourse.Values.ToList());

            // Filter out invalid schedules with conflicting sections
            foreach (var combination in sectionCombinations)
            {
                if (!HasConflict(combination))
                {
                    allSchedules.Add(combination);
                }
            }

            return allSchedules;
        }

        private List<List<Section>> GetCombinations(List<List<Section>> lists)
        {
            var result = new List<List<Section>>();

            if (lists.Count == 0)
                return result;

            if (lists.Count == 1)
            {
                foreach (var item in lists[0])
                {
                    result.Add(new List<Section> { item });
                }
                return result;
            }

            var firstList = lists[0];
            var restCombinations = GetCombinations(lists.Skip(1).ToList());

            foreach (var item in firstList)
            {
                foreach (var combination in restCombinations)
                {
                    var newList = new List<Section> { item };
                    newList.AddRange(combination);
                    result.Add(newList);
                }
            }

            return result;
        }

        private bool HasConflict(List<Section> sections)
        {
            foreach (var section1 in sections)
            {
                foreach (var section2 in sections)
                {
                    if (section1 != section2 && AreSectionsConflicting(section1, section2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AreSectionsConflicting(Section section1, Section section2)
        {
            // Check conflicts for each day
            return IsTimeOverlap(section1.Start_Sunday, section1.End_Sunday, section2.Start_Sunday, section2.End_Sunday) ||
                   IsTimeOverlap(section1.Start_Monday, section1.End_Monday, section2.Start_Monday, section2.End_Monday) ||
                   IsTimeOverlap(section1.Start_Tuesday, section1.End_Tuesday, section2.Start_Tuesday, section2.End_Tuesday) ||
                   IsTimeOverlap(section1.Start_Wednesday, section1.End_Wednesday, section2.Start_Wednesday, section2.End_Wednesday) ||
                   IsTimeOverlap(section1.Start_Thursday, section1.End_Thursday, section2.Start_Thursday, section2.End_Thursday);
        }

        private bool IsTimeOverlap(DateTime? start1, DateTime? end1, DateTime? start2, DateTime? end2)
        {
            if (start1 == null || end1 == null || start2 == null || end2 == null)
                return false;

            return start1 < end2 && start2 < end1;
        }

        public async Task<IActionResult> DisplaySchedules()
        {

            ViewData["possibleSchedules"] = possibleSchedules;


            return View();
        }


        public async Task<ActionResult> RegisterSchedule(List<int> scheduleIds, bool isFromWishlist)
        {
            Student st = _context.Students.FirstOrDefault(a => a.KeyStudent == student.KeyStudent);

            Schedule existingSchedule = _context.schedules.FirstOrDefault(s => s.students.KeyStudent == student.KeyStudent && s.Approv_Schedule == 1);
            if (existingSchedule != null)
            {
                _context.schedules.Remove(existingSchedule);
                await _context.SaveChangesAsync();
            }

            Schedule schedule = new Schedule();
            schedule.students = st;
            schedule.Approv_Schedule = isFromWishlist ? 2 : 1;

            _context.schedules.Add(schedule);
            await _context.SaveChangesAsync(); // Wait for initial save to complete

            // Associate each section with the new schedule
            foreach (var sectionId in scheduleIds)
            {
                var section = _context.sections.Find(sectionId);
                if (section != null)
                {
                    var sectionSchedule = new SectionSchedule
                    {
                        section = section,
                        schedule = schedule
                    };
                    _context.sectionSchedules.Add(sectionSchedule);
                }
            }
            await _context.SaveChangesAsync(); // Save all section schedules

            return RedirectToAction("DisplaySchedules");
        }


        public IActionResult WishlistRegisterSchedule(List<int> scheduleIds)
        {
            foreach (var scheduleId in scheduleIds)
            {
                var schedule = _context.schedules
                    .Include(s => s.sectionSchedules)
                    .ThenInclude(ss => ss.section)
                    .FirstOrDefault(s => s.IDScedule == scheduleId);

                if (schedule == null || !schedule.sectionSchedules.All(ss => ss.section.Status == "open"))
                {
                    ViewBag.Message = "no";
                    return RedirectToAction("viewWishlist");
                }
            }


            // Remove any existing approved schedules
            var existingSchedule = _context.schedules.FirstOrDefault(s => s.students.KeyStudent == student.KeyStudent && s.Approv_Schedule == 1);
            if (existingSchedule != null)
            {
                _context.schedules.Remove(existingSchedule);
                _context.SaveChanges();
            }

            // Update the wishlist schedule to be approved
            var wishlistSchedule = _context.schedules.FirstOrDefault(s => s.students.KeyStudent == student.KeyStudent && s.Approv_Schedule == 2);
            if (wishlistSchedule != null)
            {
                wishlistSchedule.Approv_Schedule = 1;
                _context.SaveChanges();
            }

            return RedirectToAction("viewWishlist");
        }


        public IActionResult PersonalPage()
        {

            var Personal = _context.Students
            .Include(s => s.studyPlan)
            .Include(s => s.schedules)
                .ThenInclude(sc => sc.sectionSchedules)
                    .ThenInclude(ss => ss.section)
                        .ThenInclude(s => s.course)
             .Include(s => s.schedules)
                .ThenInclude(sc => sc.sectionSchedules)
                    .ThenInclude(ss => ss.section)
                        .ThenInclude(s => s.Instructors)

            .FirstOrDefault(s => s.KeyStudent == student.KeyStudent);


            return View(Personal);
        }


        [HttpPost]
        public async Task<ActionResult> Wishlist(List<int> scheduleIds)
        {
            Student st = _context.Students.Where(a => a.KeyStudent == student.KeyStudent).FirstOrDefault();

            Schedule schedule = new Schedule();
            schedule.students = st;
            schedule.Approv_Schedule = 2;

            _context.schedules.Add(schedule);
            await _context.SaveChangesAsync(); // Wait for initial save to complete

            // Associate each section with the new schedule
            foreach (var sectionId in scheduleIds)
            {
                var section = _context.sections.Find(sectionId);
                if (section != null)
                {
                    var sectionSchedule = new SectionSchedule
                    {
                        section = section,
                        schedule = schedule
                    };
                    _context.sectionSchedules.Add(sectionSchedule);
                }
            }

            await _context.SaveChangesAsync(); // Save all section schedules

            return RedirectToAction("DisplaySchedules");
        }


        public IActionResult viewWishlist()
        {
            var Personal = _context.Students
            .Include(s => s.schedules)
                .ThenInclude(sc => sc.sectionSchedules)
                    .ThenInclude(ss => ss.section)
                        .ThenInclude(s => s.course)
            .Include(s => s.schedules)
                .ThenInclude(sc => sc.sectionSchedules)
                    .ThenInclude(ss => ss.section)
                        .ThenInclude(s => s.Instructors)
            .FirstOrDefault(s => s.KeyStudent == student.KeyStudent);

            return View(Personal);
        }


        [HttpPost]
        public async Task<ActionResult> RemoveSchedules(List<int> scheduleIds)
        {

            foreach (var schedule in scheduleIds)
            {
                // Find the schedule asynchronously by ID
                var foundSchedule = _context.schedules.Find(schedule);
                if (foundSchedule != null)
                {
                    // Remove associated section schedules first
                    var sectionSchedules = _context.sectionSchedules.Where(ss => ss.schedule.IDScedule == foundSchedule.IDScedule);
                    _context.sectionSchedules.RemoveRange(sectionSchedules);

                    // Remove the schedule
                    _context.schedules.Remove(foundSchedule);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the updated list view
            return RedirectToAction("viewWishlist");
        }


        public ActionResult StCompletedCreditHours()
        {
            double totalCompletedCreditHours = _context.progresses
                .Where(p => p.Student.KeyStudent == student.KeyStudent)
                .Join(_context.Courses, p => p.course.IDCRS, c => c.IDCRS, (p, c) => c.CRS_CR_HOURS)
                .Sum();
            TempData["TotalCompletedCreditHours"] = totalCompletedCreditHours;
            TempData.Keep("TotalCompletedCreditHours");

            return View();
        }
    }
}

