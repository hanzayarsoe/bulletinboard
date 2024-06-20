using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTM.Services.MailService
{
    internal class MailSetting
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
