using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IUserService
    {
        UserListViewModel Data();
        ResponseModel Create(UserViewModel model);
        UserViewModel GetUser(string id);
        ResponseModel Update(UserViewModel category);
        ResponseModel Delete(string id, string userId);
        ResponseModel EmailExists(string email);
    }
}
