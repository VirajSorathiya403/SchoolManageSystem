using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    // [CheckAccess]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public UserController(IConfiguration configuration)
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
            List<UserModel> users = new List<UserModel>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/User/getallusers");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<UserModel>>(data);
                }
                else
                {
                    ModelState.AddModelError("", $"Error: Unable to fetch users. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception occurred: {ex.Message}");
            }
            return View("UserList", users);
        }
        #endregion

        #region Add/Update
        public async Task<IActionResult> Add(int? UserId)
        {
            await LoadUserRoleList();

            if (UserId.HasValue)
            {
                var response = await _httpClient.GetAsync($"api/User/{UserId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserModel>(data);
                    return View(user);
                }
            }

            return View(new UserModel());
        }
        #endregion

        #region Save
        [HttpPost]
        public async Task<IActionResult> Save(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (user.UserId == null)
                {
                    response = await _httpClient.PostAsync("api/User/adduser", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "User added successfully!";
                        return RedirectToAction("SignIn","Auth");
                    }
                }
                else
                {
                    response = await _httpClient.PutAsync($"api/User/update/{user.UserId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "User updated successfully!";
                        return RedirectToAction("GET");
                    }
                }
            }

            await LoadUserRoleList();
            return View("Add", user);
        }
        #endregion

        #region UserRoleList
        private async Task LoadUserRoleList()
        {
            var response = await _httpClient.GetAsync("api/User/userroles");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var userRoles = JsonConvert.DeserializeObject<List<UserRoleDropDownModel>>(data);
                ViewBag.UserRoleList = userRoles;
            }
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int UserID)
        {
            var response = await _httpClient.DeleteAsync($"api/User/delete/{UserID}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "User deleted successfully!";
                return RedirectToAction("GET");
            }
            return RedirectToAction("GET");
        }
        #endregion
    }
}
