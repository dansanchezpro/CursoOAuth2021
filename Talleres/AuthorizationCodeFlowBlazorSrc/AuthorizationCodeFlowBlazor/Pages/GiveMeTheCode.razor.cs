using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AuthorizationCodeFlowBlazor.Pages
{
    public partial class GiveMeTheCode : ComponentBase
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }

        string Code, State;
        protected override void OnInitialized()
        {
            var Uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var QueryString = HttpUtility.ParseQueryString(Uri.Query);

            Code = QueryString["code"];
            State = QueryString["state"];
        }


    }
}
