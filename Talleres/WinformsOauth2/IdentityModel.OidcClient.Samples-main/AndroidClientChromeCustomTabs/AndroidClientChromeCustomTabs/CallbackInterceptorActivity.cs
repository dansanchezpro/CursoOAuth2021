
using Android.App;
using Android.Content;
using Android.OS;

namespace AndroidClientChromeCustomTabs
{
    [Activity(Label = "CallbackInterceptorActivity")]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "io.identitymodel.native",
        DataHost = "callback")]
    public class CallbackInterceptorActivity :  Activity
    {       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Finish();

            // get URI, send with mediator
            AndroidClientChromeCustomTabsApplication.Mediator.Send(Intent.DataString);

            StartActivity(typeof(MainActivity));
        }
    }
}