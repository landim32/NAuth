using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.DTO.Settings
{
    public class MailerSendSetting
    {
        public string ApiUrl { get; set; }
        public string ApiToken { get; set; }
        public string MailSender { get; set; }
    }
}
