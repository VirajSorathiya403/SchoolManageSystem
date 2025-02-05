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
        public async Task<IActionResult> GET()
        {
            List<StudentAttendanceModel> studentAttendances = new List<StudentAttendanceModel>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/StudentAttendance/getallstudentAttendances");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    studentAttendances = JsonConvert.DeserializeObject<List<StudentAttendanceModel>>(data);
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
            return View("StudentAttendanceList", studentAttendances);
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
    }
}
