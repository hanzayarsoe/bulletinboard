using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IUserRepository
    {
        UserListViewModel Data();
        ResponseModel Create(User category);
        UserViewModel GetUser(string id);
        ResponseModel Update(User category);
        ResponseModel Delete(string id, string userId);
        ResponseModel Login(string email, string password);
    }
}
