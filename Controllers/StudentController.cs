using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    [CheckAccess]
    public class StudentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
        
        #region GetStudentList
        public async Task<IActionResult> GET()
        {
            List<StudentModel> students = new List<StudentModel>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/Student/getallstudents");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    students = JsonConvert.DeserializeObject<List<StudentModel>>(data);
                }
                else
                {
                    ModelState.AddModelError("", $"Error: Unable to fetch students. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return View("StudentList", students);
        }
        #endregion
        
        #region Add/Update
        public async Task<IActionResult> Add(int? StudentId)
        {
            if (StudentId.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Student/{StudentId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var student = JsonConvert.DeserializeObject<StudentModel>(data);
                    return View(student);
                }
            }

            return View(new StudentModel());
        }
        #endregion
        
        #region Save
        [HttpPost]
        public async Task<IActionResult> Save(StudentModel student)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(student);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (student.StudentId == null)
                {                                               
                    response = await _httpClient.PostAsync("api/Student/addstudent", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Student added successfully!";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    response = await _httpClient.PutAsync($"api/Student/update/{student.StudentId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Student updated successfully!";
                        return RedirectToAction("GET");
                    }
                }
            }
            return View("Add", student);
        }
        #endregion
        
        #region Delete
        public async Task<IActionResult> Delete(int StudentId)
        {
            var response = await _httpClient.DeleteAsync($"api/Student/delete/{StudentId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Student deleted successfully!";
                return RedirectToAction("GET");
            }
            return RedirectToAction("GET");
        }
        #endregion

    }
}
