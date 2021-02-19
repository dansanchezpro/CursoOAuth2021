using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCodeFlow
{
    public class DeviceCodeFlowResponse
    {
        public string User_Code { get; set; }
        public string Device_Code { get; set; }
        public string Verification_Uri { get; set; }
        public int Expires_In { get; set; }
        public int Interval { get; set; }
        public string Message { get; set; }

    }
}
