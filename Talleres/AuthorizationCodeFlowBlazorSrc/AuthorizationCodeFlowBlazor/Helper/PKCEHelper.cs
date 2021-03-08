using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationCodeFlowBlazor.Helper
{
    public static class PKCEHelper
    {
        public static string Code_Verifier;
        public static string Code_Challenge;
        public const string Code_Challenge_Method = "S256";

        public static void GenerateCodes()
        {
            byte[] RandomBytes = new byte[43];
            var RandomGenerator = RandomNumberGenerator.Create();
            RandomGenerator.GetNonZeroBytes(RandomBytes);

            Code_Verifier = Base64UrlEncoder.Encode(RandomBytes);
            using (var Sha256 = SHA256.Create())
            {
                var ChallengeBytes = Sha256.ComputeHash(Encoding.UTF8.GetBytes(Code_Verifier));
                Code_Challenge = Base64UrlEncoder.Encode(ChallengeBytes);
            }
        }
    }
}
