using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTM.Entities.Data
{
    public class HTMLMailData
    {
        public string? EmailToId { get; set; }
        public string? EmailToName { get; set; }
        public string? EmailSubject { get; set; }
        public string? ResetLink { get; set; }
    }
}
