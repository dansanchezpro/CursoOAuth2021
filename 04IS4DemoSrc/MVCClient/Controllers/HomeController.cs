using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> CallTheApi()
        {
            var AccessToken = await HttpContext.GetTokenAsync("access_token");
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var Jsonelement = await Client.GetFromJsonAsync<JsonElement>("https://localhost:44301/Identity");
            var FormattedJsonContent = JsonSerializer.Serialize(Jsonelement, new JsonSerializerOptions { WriteIndented = true });

            return View("CallTheApi", FormattedJsonContent);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CallAdminData()
        {
            var AccessToken = await HttpContext.GetTokenAsync("access_token");
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var Jsonelement = await Client.GetFromJsonAsync<JsonElement>("https://localhost:44301/WeatherForeCast");
            var FormattedJsonContent = JsonSerializer.Serialize(Jsonelement, new JsonSerializerOptions { WriteIndented = true });

            return View("CallTheApi", FormattedJsonContent);
        }

    }
}
