using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTM.Entities.Data
{
    public class ResetPasswordModel
    {
        public string? email { get; set; }
        public string? token { get; set; }
        public string? password { get; set; }
        public string? confirmPassword { get; set; }
    }
}
