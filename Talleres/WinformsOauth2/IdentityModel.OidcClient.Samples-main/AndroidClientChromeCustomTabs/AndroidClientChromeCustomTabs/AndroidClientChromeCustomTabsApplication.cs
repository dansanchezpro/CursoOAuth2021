using Android.App;
using Android.Runtime;
using System;

namespace AndroidClientChromeCustomTabs
{
    [Application]
    public class AndroidClientChromeCustomTabsApplication : Application
    {
        public static ActivityMediator Mediator { get; } = new ActivityMediator();
        public AndroidClientChromeCustomTabsApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle,transfer)
        {
            // constructor must be available for Application instance to work
        }
    }
}