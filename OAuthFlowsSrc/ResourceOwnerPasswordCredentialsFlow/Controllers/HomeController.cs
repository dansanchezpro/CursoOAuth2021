using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ResourceOwnerPasswordCredentialsFlow.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResourceOwnerPasswordCredentialsFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        [HttpPost("get/the/token")]
        public async Task<IActionResult> GetTheToken(string userName, string password)
        {
            const string Grant_Type = "password";
            string Token_Endpoint = Configuration["OAuth:Token_Endpoint"];
            string Client_Id = Configuration["OAuth:Client_Id"];
            string Client_Secret = Configuration["OAuth:Client_Secret"];
            string Scope = Configuration["OAuth:Scope"];

            Dictionary<string, string> BodyData = new Dictionary<string, string>
            {
                { "grant_type", Grant_Type },
                { "client_id", Client_Id},
                { "client_secret", Client_Secret},
                { "scope", Scope },
                { "username", userName},
                { "password", password}
            };

            //Dictionary<string, string> BodyData = new Dictionary<string, string>
            //{
            //    { "grant_type", Grant_Type },
            //    { "client_id", Client_Id},
            //    { "client_secret", Client_Secret},
            //    { "scope", Scope + " openid profile"},
            //    { "username", userName},
            //    { "password", password},
            //    { "response_type", "token id_token"}
            //};


            HttpClient HttpClient = new HttpClient();
            var Body = new FormUrlEncodedContent(BodyData);
            var Response = await HttpClient.PostAsync(Token_Endpoint, Body);

            var Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            string ResponseContent = await Response.Content.ReadAsStringAsync();
            var JsonContent = JObject.Parse(ResponseContent).ToString();

            return View("GetTheToken", (Status, JsonContent, Response.IsSuccessStatusCode));
        }

        [HttpPost("/call/the/api")]
        public async Task<IActionResult> CallTheApi(string token)
        {
            var AccessToken = JObject.Parse(token)["access_token"].Value<string>();
            string Api_Endpoint = Configuration["OAuth:Api_Endpoint"];

            HttpClient HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var Response = await HttpClient.GetAsync(Api_Endpoint);
            (string Status, string Content) Model;
            Model.Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            Model.Content = await Response.Content.ReadAsStringAsync();
            if (Response.IsSuccessStatusCode)
            {
                var JsonElement = JsonSerializer.Deserialize<JsonElement>(Model.Content);
                Model.Content = JsonSerializer.Serialize(JsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
            return View("CallTheApi", Model);
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
    }
}
