using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace AuthorizationCodeFlowBlazor.Pages
{
    public partial class ExchangeTheCodeForAToken : ComponentBase
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IConfiguration Configuration { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public string Code { get; set; }

        string Status;
        string Content;
        bool IsSuccess;

        protected async override Task OnInitializedAsync()
        {
            const string Grant_Type = "authorization_code";
            string Code_Verifier = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "cv");

            var Uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var QueryString = HttpUtility.ParseQueryString(Uri.Query);
            Code = QueryString["code"];

            string Token_Endpoint = Configuration["OAuth:Token_Endpoint"];
            string Client_Id = Configuration["OAuth:Client_Id"];
            string Scope = Configuration["OAuth:Scope"];
            string Redirect_Uri = Configuration["OAuth:Redirect_Uri"];

            Dictionary<string, string> BodyData = new Dictionary<string, string>
            {
                { "grant_type", Grant_Type },
                { "code", Code},
                { "client_id", Client_Id},
                { "scope", Scope},
                { "code_verifier", Code_Verifier},
                { "redirect_uri", Redirect_Uri}
            };

            var HttpClient = new HttpClient();
            var Body = new FormUrlEncodedContent(BodyData);

            var Response = await HttpClient.PostAsync(Token_Endpoint, Body);
            Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            
            Content = JsonSerializer.Serialize(
                await Response.Content.ReadFromJsonAsync<JsonElement>(),
                new JsonSerializerOptions { WriteIndented = true }
                );

            IsSuccess = Response.IsSuccessStatusCode;
            if (IsSuccess)
            {
                await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", "content", Content);
            }
        }
    }
}
