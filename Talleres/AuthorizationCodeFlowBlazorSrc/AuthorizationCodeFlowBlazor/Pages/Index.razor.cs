using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCodeFlowBlazor.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        async void GetTheCode()
        {
            const string Response_Type = "code";
            Helper.PKCEHelper.GenerateCodes();
            await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", "cv", Helper.PKCEHelper.Code_Verifier);

            string Authorization_Endpoint = Configuration["OAuth:Authorization_Endpoint"];
            string Client_Id = Configuration["OAuth:Client_Id"];
            string Redirect_Uri = Configuration["OAuth:Redirect_Uri"];
            string Scope = Configuration["OAuth:Scope"];

            string URL = $"{Authorization_Endpoint}?" +
                $"response_type={Response_Type}&" +
                $"client_id={Client_Id}&" +
                $"redirect_uri={Redirect_Uri}&" +
                $"scope={Scope}&" +
                $"code_challenge={Helper.PKCEHelper.Code_Challenge}&" +
                $"code_challenge_method={Helper.PKCEHelper.Code_Challenge_Method}&" +
                $"state=EstadoDePrueba";

            NavigationManager.NavigateTo(URL);
        }
    }
}
