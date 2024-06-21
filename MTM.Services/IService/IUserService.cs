using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IUserService
    {
        UserListViewModel Data();
        UserListViewModel GetList();
        ResponseModel Create(UserViewModel model);
        UserViewModel GetUser(string id);
        ResponseModel Update(UserViewModel user);
        ResponseModel Delete(string id, string userId);
        ResponseModel GetIdByEmail(string email);
        bool CheckEmail(string email);
        ResponseModel Register(UserViewModel model);
        ResponseModel Login(string email, string password);
    }
}
