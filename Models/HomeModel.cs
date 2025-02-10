namespace SchoolManageSys.Models;

public class HomeModel
{
    public class DashboardCounts
    {
        public string Metric { get; set; }
        public int Value { get; set; }
    }
    
    public class TeacherDTO
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
    }
    
    public class AbsentSubjectDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int TotalAbsences { get; set; }
    }

    public class TeacherSubjectsDTO
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int TotalSubjects { get; set; }
        public string AssignedSubjects { get; set; } // Comma-separated list of subjects
    }

    public class Dashboard
    {
        public List<DashboardCounts> Counts { get; set; }
        public List<TeacherDTO> TeachersBirthdayToday { get; set; }
        public List<AbsentSubjectDTO> MostAbsentSubjects { get; set; }
        public List<TeacherSubjectsDTO> TeachersSubjects { get; set; }
    }
}