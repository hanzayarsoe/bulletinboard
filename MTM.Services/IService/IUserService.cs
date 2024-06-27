using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IUserService
    {
        UserListViewModel GetUserListData();
        UserListViewModel GetList(string LoginId);
        ResponseModel Create(UserListViewModel model);
        UserViewModel GetUser(string id);
        ResponseModel Update(UserViewModel user);
        ResponseModel Delete(string id, string userId);
        ResponseModel GetIdByEmail(string email);
        bool CheckEmail(string email);
        ResponseModel Register(UserViewModel model);
        ResponseModel Login(string email, string password);
        ResponseModel UpdatePassword(string id, string oldPwd, string newPwd);
    }
}
