using System;
using AuthenticationServices;
using UIKit;

namespace iOS11Client
{
            
    public class PresentationProvider : IASWebAuthenticationPresentationContextProviding
    {
        public UIWindow Anchor;

        public PresentationProvider()
        {
        }

        public IntPtr Handle => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public UIWindow GetPresentationAnchor(ASWebAuthenticationSession session)
        {
            return Anchor;
        }
    }
}
