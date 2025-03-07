using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    [CheckAccess]
    public class StudentAttendanceController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public StudentAttendanceController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
        
        #region GetStudentAttendanceList
        public async Task<IActionResult> GET(DateTime? attendanceDate)
        {
            List<StudentAttendanceModel> studentAttendances = new List<StudentAttendanceModel>();

            // If attendanceDate is not provided, set it to today's date
            if (!attendanceDate.HasValue)
            {
                attendanceDate = DateTime.Today;  // Set today's date if no date is provided
            }

            try
            {
                // Fetch all student attendance records
                HttpResponseMessage response = await _httpClient.GetAsync("api/StudentAttendance/getallstudentAttendances");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    studentAttendances = JsonConvert.DeserializeObject<List<StudentAttendanceModel>>(data);

                    // Filter by the selected date (or today's date)
                    studentAttendances = studentAttendances
                        .Where(a => a.AttendanceDate.Date == attendanceDate.Value.Date)
                        .ToList();
                }
                else
                {
                    ModelState.AddModelError("", $"Error: Unable to fetch studentAttendances. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }

            // If it's an AJAX request, return the filtered data as JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(studentAttendances); // Return JSON for AJAX requests
            }

            // Otherwise, return the view with the model

            // Otherwise, return the view with the model
            return View("StudentAttendanceList", studentAttendances); // Return the view with data for the initial page load
        }
        #endregion
        
        #region Add/Update
        public async Task<IActionResult> Add(int? AttendanceId)
        {
            await LoadTeacherList();
            await LoadSubjectList();
            await LoadStudentList();

            if (AttendanceId.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/StudentAttendance/{AttendanceId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var studentAttendance = JsonConvert.DeserializeObject<StudentAttendanceModel>(data);
                    return View(studentAttendance);
                }
            }

            return View(new StudentAttendanceModel());
        }
        #endregion

        #region Save
        [HttpPost]
        public async Task<IActionResult> Save(StudentAttendanceModel studentAttendance)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(studentAttendance);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (studentAttendance.AttendanceId == null)
                {
                    response = await _httpClient.PostAsync("api/StudentAttendance/addstudentAttendance", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "StudentAttendance added successfully!";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    response = await _httpClient.PutAsync($"api/StudentAttendance/update/{studentAttendance.AttendanceId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "StudentAttendance updated successfully!";
                        return RedirectToAction("GET");
                    }
                }
            }

            await LoadTeacherList();
            await LoadSubjectList();
            await LoadStudentList();
            return View("Add", studentAttendance);
        }
        #endregion
        
        #region StudentList
        private async Task LoadStudentList()
        {
            var response = await _httpClient.GetAsync("api/StudentAttendance/students");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var subjects = JsonConvert.DeserializeObject<List<StudentsDropDownModel>>(data);
                ViewBag.StudentList = subjects;
            }
        }
        #endregion
        
        #region TeacherList
        private async Task LoadTeacherList()
        {
            var response = await _httpClient.GetAsync("api/StudentAttendance/teachers");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var teachers = JsonConvert.DeserializeObject<List<TeachersDropDownModel>>(data);
                ViewBag.TeacherList = teachers;
            }
        }
        #endregion
        
        #region SubjectList
        private async Task LoadSubjectList()
        {
            var response = await _httpClient.GetAsync("api/StudentAttendance/subjects");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var subjects = JsonConvert.DeserializeObject<List<SubjectsDropDownModel>>(data);
                ViewBag.SubjectList = subjects;
            }
        }
        #endregion
        
        #region Delete
        public async Task<IActionResult> Delete(int AttendnaceId)
        {
            var response = await _httpClient.DeleteAsync($"api/StudentAttendance/delete/{AttendnaceId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "StudentAttendance deleted successfully!";
                return RedirectToAction("GET");
            }
            return RedirectToAction("GET");
        }
        #endregion
        
        #region SendMail
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestModel emailRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(emailRequest.Email) || string.IsNullOrEmpty(emailRequest.StudentName))
                {
                    return BadRequest(new { error = "Email and Student Name are required." });
                }

                // Fetch credentials securely (from environment variables or appsettings.json)
                var smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? "sorathiyaviraj558@gmail.com";
                var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "dlnfxanorgohepvp";

                // Configure SMTP client
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                    smtpClient.EnableSsl = true;

                    // Create email message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpEmail, "School Management"),
                        Subject = "Attendance Notification",
                        Body = $"Dear {emailRequest.StudentName},\n\nYou were absent today. Please ensure regular attendance.\n\nRegards,\nSchool Management",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(emailRequest.Email);

                    // Send email
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return Ok(new { message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to send email", details = ex.Message });
            }
        }
        #endregion
        
// Model for email request
        public class EmailRequestModel
        {
            public string StudentName { get; set; }
            public string Email { get; set; }
        }
    }
}
