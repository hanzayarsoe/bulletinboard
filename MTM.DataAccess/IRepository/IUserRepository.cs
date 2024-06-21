using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IUserRepository
    {
        UserListViewModel Data();
        UserListViewModel GetList();
        ResponseModel Create(User user);
        UserViewModel GetUser(string id);
        ResponseModel Update(User user);
        ResponseModel Delete(string id, string userId);
        ResponseModel EmailExists(string email);
        ResponseModel Register(User user);
        ResponseModel Login(string email, string password);
    }
}
