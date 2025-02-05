using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    [CheckAccess]
    public class TeacherController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TeacherController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
        
        #region GetTeacherList
        public async Task<IActionResult> GET()
        {
            List<TeacherModel> teachers = new List<TeacherModel>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/Teacher/getallteachers");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    teachers = JsonConvert.DeserializeObject<List<TeacherModel>>(data);
                }
                else
                {
                    ModelState.AddModelError("", $"Error: Unable to fetch teachers. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return View("TeacherList", teachers);
        }
        #endregion
        
        #region Add/Update
        public async Task<IActionResult> Add(int? TeacherId)
        {
            await LoadSchoolList();

            if (TeacherId.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Teacher/{TeacherId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var teacher = JsonConvert.DeserializeObject<TeacherModel>(data);
                    return View(teacher);
                }
            }

            return View(new TeacherModel());
        }
        #endregion

        #region Save
        [HttpPost]
        public async Task<IActionResult> Save(TeacherModel teacher)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(teacher);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (teacher.TeacherId == null)
                {                                               
                    response = await _httpClient.PostAsync("api/Teacher/addteacher", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Teacher added successfully!";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    response = await _httpClient.PutAsync($"api/Teacher/update/{teacher.TeacherId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Teacher updated successfully!";
                        return RedirectToAction("GET");
                    }
                }
            }
            await LoadSchoolList();
            return View("Add", teacher);
        }
        #endregion
        
        #region SchoolList
        private async Task LoadSchoolList()
        {
            var response = await _httpClient.GetAsync("api/Teacher/schools");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var schools = JsonConvert.DeserializeObject<List<SchoolDropDownModel>>(data);
                ViewBag.SchoolList = schools;
            }
        }
        #endregion
        
        #region Delete
        public async Task<IActionResult> Delete(int TeacherId)
        {
            var response = await _httpClient.DeleteAsync($"api/Teacher/delete/{TeacherId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Teacher deleted successfully!";
                return RedirectToAction("GET");
            }
            return RedirectToAction("GET");
        }
        #endregion
    }
}
