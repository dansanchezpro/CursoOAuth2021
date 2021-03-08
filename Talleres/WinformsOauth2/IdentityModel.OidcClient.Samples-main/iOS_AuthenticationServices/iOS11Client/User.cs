using System;
using System.Net.Http;
using System.Security.Claims;

namespace iOS11Client
{
    public class User
    {
        public string IdentityToken
        {
            get;
            set;
        }

        public string AccessToken
        {
            get;
            set;
        }

        public string RefreshToken
        {
            get;
            set;
        }

        public ClaimsPrincipal Claims
        {
            get;
            set;
        }

        public HttpMessageHandler Handler
        {
            get;
            set;
        }
    }
}