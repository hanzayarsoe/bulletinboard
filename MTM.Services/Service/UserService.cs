using AutoMapper;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using MTM.Services.IService;

namespace MTM.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
        }
        public ResponseModel Create(UserViewModel model)
        {
            return this._userRepository.Create(this._mapper.Map<User>(model));
        }
        public UserListViewModel Data()
        {
            return _userRepository.Data();
        }
        public UserListViewModel GetList()
        {
            return _userRepository.GetList();
        }
        public UserViewModel GetUser(string id)
        {
            return this._userRepository.GetUser(id);
        }
        public ResponseModel Update(UserViewModel model)
        {
           return this._userRepository.Update(this._mapper.Map<User>(model));
        }
        public ResponseModel Delete(string id, string userId)
        {
            return this._userRepository.Delete(id, userId);
        }

        public ResponseModel GetIdByEmail(string email)
        {
            return this._userRepository.GetIdByEmail(email);
        }

        public bool CheckEmail(string email)
        {
            return this._userRepository.CheckEmail(email);
        }

        public ResponseModel Register(UserViewModel model)
        {
            return this._userRepository.Register(this._mapper.Map<User>(model));
        }
        public ResponseModel Login(string email, string password)
        {
            return this._userRepository.Login(email, password);
        }
    }
}
