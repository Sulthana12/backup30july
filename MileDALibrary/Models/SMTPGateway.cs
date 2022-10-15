using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileDALibrary.Models
{
    public class SMTPGateway
    {
        public string OfficialFromEmailID { get; set; }

        public string OfficialFromEmailIDPassword { get; set; }

        public string Gateway { get; set; }

        public bool RequireSSL { get; set; }

        public bool RequireTLS { get; set; }

        public bool RequireAuthentication { get; set; }

        public int Port { get; set; }
    }
}
