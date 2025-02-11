using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new HomeModel.Dashboard
            {   
                Counts = new List<HomeModel.DashboardCounts>(),
                TeachersBirthdayToday = new List<HomeModel.TeacherDTO>(),
                MostAbsentSubjects = new List<HomeModel.AbsentSubjectDTO>(),
                TeachersSubjects = new List<HomeModel.TeacherSubjectsDTO>(),
            };

            // ✅ Use `await using` for proper async resource disposal
            await using var connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString"));
            await connection.OpenAsync();

            await using var command = new SqlCommand("usp_GetSchoolDashboardData", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await using var reader = await command.ExecuteReaderAsync();

            // ✅ Fetch counts
            while (await reader.ReadAsync())
            {
                dashboardData.Counts.Add(new HomeModel.DashboardCounts
                {
                    Metric = reader["Metric"].ToString(),
                    Value = Convert.ToInt32(reader["Value"])
                });
            }

            // ✅ Fetch teachers with birthdays today
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    dashboardData.TeachersBirthdayToday.Add(new HomeModel.TeacherDTO
                    {
                        TeacherId = Convert.ToInt32(reader["TeacherId"]),
                        TeacherName = reader["TeacherName"].ToString(),
                        MobileNo = reader["MobileNo"].ToString(),
                        Email = reader["Email"].ToString()
                    });
                }
            }

            // ✅ Fetch subjects with most absences
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    dashboardData.MostAbsentSubjects.Add(new HomeModel.AbsentSubjectDTO
                    {
                        SubjectId = Convert.ToInt32(reader["SubjectId"]),
                        SubjectName = reader["SubjectName"].ToString(),
                        TeacherName = reader["TeacherName"].ToString(),
                        AttendanceDate = Convert.ToDateTime(reader["AttendanceDate"]),
                        TotalAbsences = Convert.ToInt32(reader["TotalAbsences"])
                    });
                }
            }
            // dashboardData.MostAbsentSubjects = dashboardData.MostAbsentSubjects
            //     .OrderByDescending(s => s.TotalAbsences)
            //     .ThenByDescending(s => s.AttendanceDate)
            //     .ThenBy(s => s.TeacherName)
            //     .ToList();
            // ✅ No need for extra sorting if handled in SQL
            
            // ✅ Fetch teachers with assigned subject
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    dashboardData.TeachersSubjects.Add(new HomeModel.TeacherSubjectsDTO
                    {
                        TeacherId = Convert.ToInt32(reader["TeacherId"]),
                        TeacherName = reader["TeacherName"].ToString(),
                        TotalSubjects = Convert.ToInt32(reader["TotalSubjects"]),
                        AssignedSubjects = reader["AssignedSubjects"].ToString() // Comma-separated subjects
                    });
                }
            }
            
            return View("Index", dashboardData);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
