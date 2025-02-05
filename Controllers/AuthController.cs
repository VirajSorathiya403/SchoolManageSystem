using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManageSys.Models;

namespace SchoolManageSys.Controllers
{
    public class AuthController : Controller
        {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
    
        #region SignIn
        [HttpGet]
        public IActionResult SignIn()
        {
            var userJson = HttpContext.Session.GetString("UserDetails");
    
            if (!string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Index", "Home");
            }
            return View("SignIn",new UserAuthModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserAuthModel userAuthModel)
        {
            if (ModelState.IsValid)
            {       
                try
                {
                    var json = JsonConvert.SerializeObject(userAuthModel);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PostAsync("api/User/signin", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                        if (result != null && result["Message"]?.ToString() == "Sign in Successfully")
                        {
                            var userDetailsJson = result["UserDetails"]?.ToString();
                            var userDetails = JsonConvert.DeserializeObject<UserModel>(userDetailsJson);

                            // Log user details to console
                            Console.WriteLine($"User logged in successfully. UserDetails: {JsonConvert.SerializeObject(userDetails)}");

                            // Store user details in session
                            HttpContext.Session.SetString("UserDetails", JsonConvert.SerializeObject(userDetails));
                            TempData["Message"] = "Sign-in successful!"; // Message for Index page
                            return RedirectToAction("Index", "Home"); // Redirect after success
                        }
                        else
                        {
                            TempData["Error"] = result["Message"]?.ToString() ?? "Invalid credentials"; // Message for SignIn page
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Unable to connect to the server. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Exception: {ex.Message}";
                }
            }

            return View(userAuthModel); // Return the SignIn page with error message
        }
        
        #endregion

        #region SignOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                var userDetailsJson = HttpContext.Session.GetString("UserDetails");
                if (!string.IsNullOrEmpty(userDetailsJson))
                {
                    var userDetails = JsonConvert.DeserializeObject<UserAuthModel>(userDetailsJson);
                    var json = JsonConvert.SerializeObject(userDetails);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await _httpClient.PostAsync("api/User/signout", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                        if (result != null && result["Message"]?.ToString() == "Sign out Successfully")
                        {
                            HttpContext.Session.Clear();
                            TempData["Message"] = "Signed out successfully!"; // Message for SignIn page
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Error"] = result["Message"]?.ToString() ?? "Error signing out"; // Message for SignIn page
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Unable to connect to the server. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
