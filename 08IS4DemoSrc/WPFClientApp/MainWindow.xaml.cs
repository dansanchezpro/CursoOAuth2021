using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string Authority = "https://localhost:44300";
        const string Api_Endpoint = "https://localhost:44301/identity";
        TokenResponse TokenResponse;
        public MainWindow()
        {
            InitializeComponent();
        }
        async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(string authority)
        {
            var HttpClient = new HttpClient();
            return await HttpClient.GetDiscoveryDocumentAsync(authority);
        }

        async Task<TokenResponse> RequestTokenAsync(string tokenEndpoint)
        {
            var Client = new HttpClient();
            return await Client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = "ClientApp",
                    ClientSecret = "Secret",
                    Scope = "weatherapi.read weatherapi.write"
                });
        }
        async Task<TokenResponse> RequestPasswordTokenAsync(string tokenEndpoint)
        {
            var Client = new HttpClient();
            return await Client.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = "ClientApp",
                    ClientSecret = "Secret",
                    UserName = "alice",
                    Password = "alice",
                    Scope = "weatherapi.read weatherapi.write"
                });
        }
        private async void GetTheToken_Click(object sender, RoutedEventArgs e)
        {
            var DiscoveryDocumentResponse = await GetDiscoveryDocumentAsync(Authority);
            if (DiscoveryDocumentResponse.IsError)
            {
                Messages.Text = DiscoveryDocumentResponse.Error;
            }
            else
            {
                TokenResponse = await RequestTokenAsync(DiscoveryDocumentResponse.TokenEndpoint);
                if (TokenResponse.IsError)
                {
                    Messages.Text = TokenResponse.Error;
                }
                else
                {
                    Messages.Text = JsonSerializer.Serialize(TokenResponse.Json, new JsonSerializerOptions { WriteIndented = true });
                }
            }
        }
        private async void CallTheApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var Client = new HttpClient();
                Client.SetBearerToken(TokenResponse.AccessToken);
                var Response = await Client.GetAsync(Api_Endpoint);
                if (Response.IsSuccessStatusCode)
                {
                    Messages.Text = JsonSerializer.Serialize(await Response.Content.ReadFromJsonAsync<JsonElement>(), new JsonSerializerOptions { WriteIndented = true });
                }
                else
                {
                    Messages.Text = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {

                Messages.Text = ex.Message;
            }

        }

        private async void GetThePasswordToken_Click(object sender, RoutedEventArgs e)
        {
            var DiscoveryDocumentResponse = await GetDiscoveryDocumentAsync(Authority);
            if (DiscoveryDocumentResponse.IsError)
            {
                Messages.Text = DiscoveryDocumentResponse.Error;
            }
            else
            {
                TokenResponse = await RequestPasswordTokenAsync(DiscoveryDocumentResponse.TokenEndpoint);
                if (TokenResponse.IsError)
                {
                    Messages.Text = TokenResponse.Error;
                }
                else
                {
                    Messages.Text = JsonSerializer.Serialize(TokenResponse.Json, new JsonSerializerOptions { WriteIndented = true });
                }
            }
        }
    }
}
