using Android.App;
using Android.OS;
using Android.Widget;
using IdentityModel.OidcClient;
using Java.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using Android.Content;

namespace AndroidClientChromeCustomTabs
{
    /// <summary>
    /// Important note: the Chrome custom tab intent closes when you navigate away from it.  So, when 
    /// the result is parsed the IWebView implementation navigates back to MainActivity.  Therefore, LaunchMode
    /// is set to SingleTask to avoid multiple instances being created. 
    /// 
    /// Alternatively, you can navigate to another activity (or to MainActivity with standard launchmode), as long as you
    /// make sure to keep the results of the login stored somewhere you can access it.  
    /// </summary>
    [Activity(Label = "AndroidClientChromeCustomTabs", MainLauncher = true, Icon = "@drawable/icon", 
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        private OidcClient _oidcClient;
        private HttpClient _apiClient;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
             // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += Button_Click;            
            Button btnCallApi = FindViewById<Button>(Resource.Id.button1);
            btnCallApi.Click += BtnCallApi_Click;
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            var authority = "https://demo.identityserver.io";

            var options = new OidcClientOptions
            {
                Authority = authority,
                ClientId = "native.hybrid",
                ClientSecret = "xoxo",
                Scope = "openid profile api",
                RedirectUri = "io.identitymodel.native://callback",
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Browser = new ChromeCustomTabsWebView(this)
            };

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync();

            var txtResult = FindViewById<EditText>(Resource.Id.editText1);

            if (!string.IsNullOrEmpty(result.Error))
            {             
                txtResult.Text = result.Error;
                return;
            }

            var sb = new StringBuilder(128);
            foreach (var claim in result.User.Claims)
            {
                sb.Append(string.Format("{0}: {1}\n", claim.Type, claim.Value));
            }

            //sb.Append(string.Format("\n{0}: {1}\n", "refresh token", result.RefreshToken));
            sb.Append(string.Format("\n{0}: {1}\n", "access token", result.AccessToken));

            txtResult.Text = sb.ToString();

            // commented out since no refresh token
            //_apiClient = new HttpClient(result.Handler);
            _apiClient = new HttpClient();
            // added since not using result.Handler above
            _apiClient.SetBearerToken(result.AccessToken);
            _apiClient.BaseAddress = new Uri("https://api.identityserver.io/");
        }

        private async void BtnCallApi_Click(object sender, EventArgs e)
        {
            if (_apiClient == null)
            {
                return;
            }

            var txtResult = FindViewById<EditText>(Resource.Id.editText1);

            var result = await _apiClient.GetAsync("identity");
            if (result.IsSuccessStatusCode)
            {
                txtResult.Text = JArray.Parse(await result.Content.ReadAsStringAsync()).ToString();
            }
            else
            {
                txtResult.Text = result.ReasonPhrase;
            }
        }         
    }
}

