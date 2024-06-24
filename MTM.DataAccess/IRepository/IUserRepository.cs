using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IUserRepository
    {
        UserListViewModel Data();
        UserListViewModel GetList(String LoginId);
        ResponseModel Create(User user);
        UserViewModel GetUser(string id);
        ResponseModel Update(User user);
        ResponseModel Delete(string id, string userId);
        ResponseModel GetIdByEmail(string email);
        bool CheckEmail(string email);
        ResponseModel Register(User user);
        ResponseModel Login(string email, string password);
        ResponseModel UpdatePassword(string id, string oldPwd, string newPwd);
        ResponseModel UploadUser(string filePath);
    }
}
