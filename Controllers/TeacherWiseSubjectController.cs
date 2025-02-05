using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    [CheckAccess]
    public class TeacherWiseSubjectController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TeacherWiseSubjectController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
        
        #region GetUserList
        public async Task<IActionResult> GET()
        {
            List<TeacherWiseSubjectModel> teacherWiseSubjects = new List<TeacherWiseSubjectModel>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/TeacherWiseSubject/getallteacherWiseSubjects");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    teacherWiseSubjects = JsonConvert.DeserializeObject<List<TeacherWiseSubjectModel>>(data);
                }
                else
                {
                    ModelState.AddModelError("", $"Error: Unable to fetch teacherWiseSubjects. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return View("TeacherWiseSubjectList", teacherWiseSubjects);
        }
        #endregion
        
        #region Add
        public async Task<IActionResult> Add()
        {
            await LoadTeacherList();
            await LoadSubjectList();
            await LoadAcademicYearList();
            return View(new TeacherWiseSubjectModel());
        }
        #endregion
        
        #region Save
        [HttpPost]
        public async Task<IActionResult> Save(TeacherWiseSubjectModel teacherWiseSubject)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(teacherWiseSubject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Since we are only handling the add functionality
                var response = await _httpClient.PostAsync("api/TeacherWiseSubject/addteacherWiseSubject", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "TeacherWiseSubject added successfully!";
                    return RedirectToAction("GET");
                }
            }

            await LoadTeacherList();
            await LoadSubjectList();
            await LoadAcademicYearList();
            return View("Add", teacherWiseSubject); // Return to the Add view with validation errors
        }
        #endregion
        
        #region TeacherList
        private async Task LoadTeacherList()
        {
            var response = await _httpClient.GetAsync("api/TeacherWiseSubject/teachers");
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
            var response = await _httpClient.GetAsync("api/TeacherWiseSubject/subjects");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var subjects = JsonConvert.DeserializeObject<List<SubjectsDropDownModel>>(data);
                ViewBag.SubjectList = subjects;
            }
        }
        #endregion
        
        #region AcademicYear
        private async Task LoadAcademicYearList()
        {
            var response = await _httpClient.GetAsync("api/TeacherWiseSubject/academicYears");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var academicYears = JsonConvert.DeserializeObject<List<AcademicYearsDropDownModel>>(data);
                ViewBag.AcademicYearList = academicYears;
            }
        }
        #endregion
        
        #region Delete
        public async Task<IActionResult> Delete(int TeacherWiseSubjectID)
        {
            var response = await _httpClient.DeleteAsync($"api/TeacherWiseSubject/delete/{TeacherWiseSubjectID}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "TeacherWiseSubject deleted successfully!";
                return RedirectToAction("GET");
            }
            return RedirectToAction("GET");
        }
        #endregion
    }
}
