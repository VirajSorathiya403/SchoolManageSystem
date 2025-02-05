namespace SchoolManageSys.Models;

public class TeacherWiseSubjectModel
{
    public int? TeacherWiseSubjectId { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYear { get; set; }
}
    
public class TeachersDropDownModel
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
}
    
public class SubjectsDropDownModel
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
}
    
public class AcademicYearsDropDownModel
{
    public int AcademicYearId { get; set; }
    public string YearName { get; set; }
}