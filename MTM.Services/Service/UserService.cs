using AutoMapper;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using MTM.Services.IService;

namespace MTM.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository categoryRepository, IMapper mapper)
        {
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
        }

        public ResponseModel Create(UserViewModel model)
        {
            return this._categoryRepository.Create(this._mapper.Map<User>(model));
        }

        public UserListViewModel Data()
        {
            return _categoryRepository.Data();
        }

        public UserViewModel GetCategory(string id)
        {
            return this._categoryRepository.GetUser(id);
        }

        public ResponseModel Update(UserViewModel model)
        {
           return this._categoryRepository.Update(this._mapper.Map<User>(model));
        }

        public ResponseModel Delete(string id, string userId)
        {
            return this._categoryRepository.Delete(id, userId);
        }
    }
}
