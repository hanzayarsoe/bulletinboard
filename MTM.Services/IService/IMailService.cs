using MTM.Entities.Data;

namespace MTM.Services.IService
{
    public interface IMailService
    {
        bool SendHTMLMail(HTMLMailData htmlMailData);
    }
}
