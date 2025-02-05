using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    [CheckAccess]
    public class SubjectController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public SubjectController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
        
        #region GetSubjectList
        public async Task<IActionResult> GET()
        {
            List<SubjectModel> subjects = new List<SubjectModel>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/Subject/getallsubjects");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    subjects = JsonConvert.DeserializeObject<List<SubjectModel>>(data);
                }
                else
                {
                    ModelState.AddModelError("", $"Error: Unable to fetch subjects. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return View("SubjectList", subjects);
        }
        #endregion
        
        #region Add/Update
        public async Task<IActionResult> Add(int? SubjectId)
        {
            if (SubjectId.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/Subject/{SubjectId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var subject = JsonConvert.DeserializeObject<SubjectModel>(data);
                    return View(subject);
                }
            }

            return View(new SubjectModel());
        }
        #endregion
        
        #region Save
        [HttpPost]
        public async Task<IActionResult> Save(SubjectModel subject)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(subject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (subject.SubjectId == null)
                {                                               
                    response = await _httpClient.PostAsync("api/Subject/addsubject", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Subject added successfully!";
                        return RedirectToAction("GET");
                    }
                }
                else
                {
                    response = await _httpClient.PutAsync($"api/Subject/update/{subject.SubjectId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Subject updated successfully!";
                        return RedirectToAction("GET");
                    }
                }
            }
            return View("Add", subject);
        }
        #endregion
        
        #region Delete
        public async Task<IActionResult> Delete(int SubjectId)
        {
            var response = await _httpClient.DeleteAsync($"api/Subject/delete/{SubjectId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Subject deleted successfully!";
                return RedirectToAction("GET");
            }
            return RedirectToAction("GET");
        }
        #endregion
    }
}
