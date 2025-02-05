using System.ComponentModel.DataAnnotations;

namespace SchoolManageSys.Models;

public class TeacherModel
{
    public int? TeacherId { get; set; }
    public string TeacherName { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName{ get; set; }
    
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    public decimal Salary { get; set; }
    public int  ExperienceYears { get; set; }
    public DateTime? JoiningDate { get; set; }
    public string Gender { get; set; }
}
    
public class SchoolDropDownModel
{
    public int SchoolId { get; set; }
    public string SchoolName { get; set; }
}