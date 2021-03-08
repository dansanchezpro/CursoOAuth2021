using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using IdentityModel.OidcClient.Browser;

namespace UwpSample
{
    class SystemBrowser : IBrowser
    {
        static TaskCompletionSource<BrowserResult> inFlightRequest;
        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            inFlightRequest?.TrySetCanceled();
            inFlightRequest = new TaskCompletionSource<BrowserResult>();

            var res = Launcher.LaunchUriAsync(new Uri(options.StartUrl));

            return inFlightRequest.Task;
        }

        public static void ProcessResponse(Uri responseData)
        {
            var result = new BrowserResult
            {
                Response = responseData.OriginalString,
                ResultType = BrowserResultType.Success
            };
            
            inFlightRequest.SetResult(result);
            inFlightRequest = null; 
        }
    }
}
