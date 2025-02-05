using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace SchoolManageSys.Models;

public class StudentModel
{
    public int? StudentId { get; set; }
    public string StudentName { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public string ParentNo { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; }
    public int EnrollmentNumber { get; set; }
}