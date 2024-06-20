using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTM.DataAccess.IRepository
{
    public interface IMailRepository
    {
        bool sendMail(string email, string resetLink);
    }
}
