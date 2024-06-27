using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IUserRepository
    {
        UserListViewModel GetUserListData();
        UserListViewModel GetList(String LoginId);
        ResponseModel Create(List<User> user);
        UserViewModel GetUser(string id);
        ResponseModel Update(User user);
        ResponseModel Delete(string id, string userId);
        ResponseModel GetIdByEmail(string email);
        bool CheckEmail(string email);
        ResponseModel Register(User user);
        ResponseModel Login(string email, string password);
        ResponseModel UpdatePassword(string id, string oldPwd, string newPwd);
    }
}
