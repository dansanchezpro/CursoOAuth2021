using CodeFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;

        public HomeController(ILogger<HomeController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        [HttpGet("/get/the/code")]
        public IActionResult GetTheCode()
        {
            const string State = "ThisIsMyStateValue";
            const string Response_Mode = "form_post";
            string Authorization_Endpoint = Configuration["OAuth:Authorization_Endpoint"];
            string Response_Type = "code id_token";
            string Client_Id = Configuration["OAuth:Client_Id"];
            string Redirect_Uri = Configuration["OAuth:Redirect_Uri"];
            string Scope = Configuration["OAuth:Scope"];
            string Nonce = Guid.NewGuid().ToString();

            string URL = $"{Authorization_Endpoint}?" +
                $"response_type={Response_Type}&" +
                $"client_id={Client_Id}&" +
                $"redirect_uri={Redirect_Uri}&" +
                $"scope={Scope}&" +
                $"state={State}&" +
                $"code_challenge={CodeFlowWithPKCE.Helpers.PKCEHelper.Code_Challenge}&" +
                $"code_challenge_method={CodeFlowWithPKCE.Helpers.PKCEHelper.Code_Challenge_Method}&" +
                $"nonce={Nonce}&" +
                $"response_mode={Response_Mode}";

            return Redirect(URL);
        }

        [HttpPost("/give/me/the/code")]
        public IActionResult GiveMeTheCode(string code, string state, string id_token)
        {
            return View((code, state, id_token));
        }

        [HttpGet("/exchange/the/code/for/a/token")]
        public async Task<IActionResult> ExchangeTheCodeForAToken(string code, string state)
        {
            const string Grant_Type = "authorization_code";
            string Token_Endpoint =
                Configuration["OAuth:Token_Endpoint"];
            string Redirect_Uri =
                Configuration["OAuth:Redirect_Uri"];
            string Client_Id =
                Configuration["OAuth:Client_Id"];
            string Client_Secret =
                Configuration["OAuth:Client_Secret"];
            string Scope =
                Configuration["OAuth:Scope"];

            Dictionary<string, string> BodyData =
                new Dictionary<string, string>
                {
                    { "grant_type", Grant_Type},
                    { "code", code},
                    { "redirect_uri", Redirect_Uri},
                    { "client_id", Client_Id},
                    { "client_secret", Client_Secret},
                    { "scope", Scope},
                    { "code_verifier", CodeFlowWithPKCE.Helpers.PKCEHelper.Code_Verifier}
                };
            var HttpClient = new HttpClient();
            var Body = new FormUrlEncodedContent(BodyData);
            var Response = await HttpClient.PostAsync(Token_Endpoint, Body);
            var Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            string ResponseContent = await Response.Content.ReadAsStringAsync();

            var JsonResponse = JObject.Parse(ResponseContent).ToString();

            return View("ExchangeTheCodeForAToken",
                (Status, JsonResponse, Response.IsSuccessStatusCode));

        }

        [HttpPost("/call/the/api")]
        public async Task<IActionResult> CallTheApi(string token)
        {
            var AccessToken =
                JObject.Parse(token)["access_token"].Value<string>();
            string Api_Endpoint =
                Configuration["OAuth:Api_Endpoint"];
            var HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
            var Response = await HttpClient.GetAsync(Api_Endpoint);
            
            (string Status, string Content) Model;
            Model.Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            if (Response.IsSuccessStatusCode)
            {
                string ResponseContent = await Response.Content.ReadAsStringAsync();
                var JsonElement = JsonSerializer.Deserialize<JsonElement>(ResponseContent);
                Model.Content = JsonSerializer.Serialize(JsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                Model.Content = await Response.Content.ReadAsStringAsync();
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
