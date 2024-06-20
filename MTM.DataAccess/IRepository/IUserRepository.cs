using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IUserRepository
    {
        UserListViewModel Data();
        UserListViewModel GetList();
        ResponseModel Create(User category);
        UserViewModel GetUser(string id);
        ResponseModel Update(User category);
        ResponseModel Delete(string id, string userId);
        ResponseModel EmailExists(string email);
        ResponseModel Login(string email, string password);
    }
}
