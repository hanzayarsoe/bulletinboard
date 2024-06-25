using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IMailService
    {
        Task<bool> SendHTMLMail(HTMLMailData htmlMailData);
    }
}
