using System;
using System.Text;
using IdentityModel.OidcClient;

using UIKit;
using Foundation;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
	public partial class ViewController : UIViewController
	{
		SafariServices.SFSafariViewController safari;
		OidcClient _client;
		AuthorizeState _state;
		HttpClient _apiClient;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CallApiButton.Enabled = false;

			LoginButton.TouchUpInside += LoginButton_TouchUpInside;
			CallApiButton.TouchUpInside += CallApiButton_TouchUpInside;
		}

		async void LoginButton_TouchUpInside (object sender, EventArgs e)
		{
			var options = new OidcClientOptions
			{
				Authority = "https://demo.identityserver.io",
				ClientId = "native.hybrid",
				Scope = "openid profile email api",
				RedirectUri = "io.identitymodel.native://callback",

				ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
			};

			_client = new OidcClient (options);
			_state = await _client.PrepareLoginAsync ();

			AppDelegate.CallbackHandler = HandleCallback;
			safari = new SafariServices.SFSafariViewController (new NSUrl (_state.StartUrl));

			this.PresentViewController (safari, true, null);
		}

		async void CallApiButton_TouchUpInside (object sender, EventArgs e)
		{
			if (_apiClient == null) {
				return;
			}

			var result = await _apiClient.GetAsync ("test");

			var content = await result.Content.ReadAsStringAsync ();

			if (!result.IsSuccessStatusCode) {
				OutputTextView.Text = result.ReasonPhrase + "\n\n" + content;
				return;
			}

			OutputTextView.Text = JArray.Parse (content).ToString ();
		}

		async void HandleCallback (string url)
		{
			await safari.DismissViewControllerAsync (true);

			var result = await _client.ProcessResponseAsync (url, _state);

			if (result.IsError)
			{
				OutputTextView.Text = result.Error;
				return;
			}

			var sb = new StringBuilder (128);
			foreach (var claim in result.User.Claims) {
				sb.AppendFormat ("{0}: {1}\n", claim.Type, claim.Value);
			}

			sb.AppendFormat ("\n{0}: {1}\n", "refresh token", result?.RefreshToken ?? "none");
			sb.AppendFormat ("\n{0}: {1}\n", "access token", result.AccessToken);

			OutputTextView.Text = sb.ToString ();

			_apiClient = new HttpClient ();
			_apiClient.SetBearerToken (result.AccessToken);
			_apiClient.BaseAddress = new Uri ("https://demo.identityserver.io/api/");

			CallApiButton.Enabled = true;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
