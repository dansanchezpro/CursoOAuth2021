using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceCodeFlow
{
    class Program
    {
        const string DeviceCode_Endpoint = "https://login.microsoftonline.com/620af200-d1ef-40af-a3d8-3c79dee81f3d/oauth2/v2.0/devicecode";
        const string Token_Endpoint = "https://login.microsoftonline.com/620af200-d1ef-40af-a3d8-3c79dee81f3d/oauth2/v2.0/token";
        const string Client_Id = "9e87f0cb-7989-4c4b-a1f5-2b4bbf9105a9";
        const string Scope = "https://graph.microsoft.com/User.Read";
        const string Api_Endpoint = "https://graph.microsoft.com/v1.0/me";
        static void Main(string[] args)
        {
            Console.Clear();
            var Response = GetTheCode().Result;
            if (Response.DeviceCode != null)
            {
                //Console.WriteLine($"Código recibido: {Response.DeviceCode.Device_Code}");
                var ValidationResponse = WaitTheUserToValidateTheCode(Response).Result;
                if (ValidationResponse.Validated)
                {
                    CallTheApi(ValidationResponse.JsonContent).Wait();
                }
                else
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ValidationResponse.JsonContent);
                }
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Response.JsonContent);
            }
        }
        static async Task<(string JsonContent, DeviceCodeFlowResponse DeviceCode)> GetTheCode()
        {
            (string JsonContent, DeviceCodeFlowResponse Response) Result = default;

            Console.WriteLine("Paso 1: Recuperar el código del dispositivo:");
            Console.WriteLine("Presiona <enter> para continuar.");
            Console.ReadLine();

            Dictionary<string, string> BodyData = new Dictionary<string, string>
            {
                { "client_id", Client_Id },
                { "scope", Scope}
            };
            var HttpClient = new HttpClient();
            var Body = new FormUrlEncodedContent(BodyData);

            var Response = await HttpClient.PostAsync(DeviceCode_Endpoint, Body);

            string ResponseContent = await Response.Content.ReadAsStringAsync();

            Result.JsonContent = JObject.Parse(ResponseContent).ToString();
            if (Response.IsSuccessStatusCode)
            {
                Result.Response = JsonSerializer.Deserialize<DeviceCodeFlowResponse>(
                    ResponseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            return Result;
        }

        static async Task<(bool Validated, string JsonContent)> WaitTheUserToValidateTheCode((string JsonContent, DeviceCodeFlowResponse DeviceCode) response)
        {
            Console.Clear();
            Console.WriteLine("Paso 2: Esperar a que el usuario valide el código.");
            Console.WriteLine();
            Console.WriteLine("Esta es la respuesta del endpoint /devicecode");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(response.JsonContent);
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("Para iniciar sesión, utiliza un navegador web para abrir la página ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(response.DeviceCode.Verification_Uri);
            Console.ResetColor();
            Console.Write(" y proporciona ahí el código ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(response.DeviceCode.User_Code);
            Console.ResetColor();
            Console.WriteLine(" para autenticarte.");
            Console.WriteLine();
            Console.Write("Esperando");

            return await PollForUpdates(response.DeviceCode);
        }

        private static async Task<(bool Validated, string JsonContent)> PollForUpdates(DeviceCodeFlowResponse response)
        {
            (bool Validated, string JsonContent) Result = (false, null);

            CancellationTokenSource PollingCancellationToken = new CancellationTokenSource();
            int ElapsedTime = 0;
            while (!PollingCancellationToken.IsCancellationRequested)
            {
                ElapsedTime += response.Interval;
                if (ElapsedTime <= response.Expires_In)
                {
                    await Task.Delay(response.Interval * 1000);
                    try
                    {
                        Console.Write(".");
                        var ValidationResponse = await GetTheAccessTokenIfTheUserHasValidatedTheCode(response.Device_Code);
                        if (ValidationResponse.StatusCode == HttpStatusCode.OK)
                        {
                            PollingCancellationToken.Cancel();
                            Result.Validated = true;
                            Result.JsonContent = ValidationResponse.ResponseContent;
                        }
                        else
                        {
                            if (!(ValidationResponse.ResponseContent.Contains("authorization_pending") ||
                                ValidationResponse.ResponseContent.Contains("slow_down")))
                            {
                                PollingCancellationToken.Cancel();
                                Result.JsonContent = ValidationResponse.ResponseContent;
                            }
                        }
                    }
                    catch
                    {
                        //podriamos implementar una bitacora
                        PollingCancellationToken.Cancel();
                    }
                }
                else
                {
                    PollingCancellationToken.Cancel();
                }
            }
            return Result;
        }

        static async Task<(HttpStatusCode StatusCode, string ResponseContent)> GetTheAccessTokenIfTheUserHasValidatedTheCode(string device_Code)
        {
            const string Grant_Type = "urn:ietf:params:oauth:grant-type:device_code";
            var HttpClient = new HttpClient();
            Dictionary<string, string> BodyData = new Dictionary<string, string>
            {
                { "grant_type", Grant_Type },
                { "client_id", Client_Id},
                { "device_code", device_Code }
            };
            var Body = new FormUrlEncodedContent(BodyData);
            var Response = await HttpClient.PostAsync(Token_Endpoint, Body);

            return (Response.StatusCode, await Response.Content.ReadAsStringAsync());
        }

        static async Task CallTheApi(string response)
        {
            var JsonElement = JsonSerializer.Deserialize<JsonElement>(response);
            response = JsonSerializer.Serialize(JsonElement, new JsonSerializerOptions { WriteIndented = true });
            Console.Clear();
            Console.WriteLine("Paso 3: Acceder al recurso protegido con el Token de Acceso");
            Console.WriteLine();
            Console.WriteLine("Respuesta recibida:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(response);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Presiona <enter> para invocar a la API");
            Console.ReadLine();

            var AccessToken = JsonElement.GetProperty("access_token").ToString();
            var HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var Response = await HttpClient.GetAsync(Api_Endpoint);
            var Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            var Content = await Response.Content.ReadAsStringAsync();

            Console.WriteLine("Este es el resultado de la llamada a la API");
            Console.WriteLine();
            if (Response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                JsonElement = JsonSerializer.Deserialize<JsonElement>(Content);
                Content = JsonSerializer.Serialize(JsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Response);
            }
            Console.WriteLine(Status);
            Console.WriteLine(Content);
        }
    }
}
