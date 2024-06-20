using MTM.CommonLibrary;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        #region List
        public UserListViewModel Data()
        {
            UserListViewModel list = new UserListViewModel();
            try
            {
                using (var context = new MTMContext())
                {
                    list.UserList = (from data in context.Users
                                     where data.IsActive == true & data.IsDeleted == false
                                     select new UserViewModel
                                     {
                                         Id = data.Id,
                                         FirstName = data.FirstName,
                                         IsActive = data.IsActive ? true : false,
                                         CreatedDate = data.CreatedDate,
                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return list;
        }
        #endregion

        #region Create
        public ResponseModel Create(User user)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var checkExist = context.Users.FirstOrDefault(c => c.Email == user.Email);
                    if (checkExist != null)
                    {
                        response.ResponseType = Message.EXIST;
                        response.ResponseMessage = string.Format(Message.ALREADY_EXIST, user.Email);
                    }
                    else
                    {
                        context.Users.Add(user);
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, user.FirstName, "created");
                    }
                }
            }
            catch (Exception ex)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }
        #endregion

        #region GetUser
        public UserViewModel GetUser(string id)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using (var context = new MTMContext())
                {
                    model = (from data in context.Users
                             where
                             data.Id == id &
                             data.IsActive == true &
                             data.IsDeleted == false
                             select new UserViewModel
                             {
                                 Id = data.Id,
                                 FirstName = data.FirstName,
                                 IsActive = data.IsActive
                             }).First();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return model;
        }
        #endregion

        #region Update
        public ResponseModel Update(User user)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    User isExist = (from data in context.Users
                                    where
                                        data.Id != user.Id &
                                        data.FirstName == user.FirstName &
                                        data.IsDeleted == false
                                    select data
                                       ).First();

                    if (isExist != null)
                    {
                        context.Users.Update(user);
                        context.SaveChanges();
                        response.ResponseType = Message.EXIST;
                        response.ResponseMessage = string.Format(Message.ALREADY_EXIST, user.FirstName);
                    }
                    else
                    {
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, user.FirstName, "updated");
                    }
                }
            }
            catch (Exception ex)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }
        #endregion

        #region Delete
        public ResponseModel Delete(string id, string userId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var userModel = context.Users.FirstOrDefault(c => c.Id == id);
                    if (userModel == null)
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST, userModel?.FirstName);
                    }
                    else
                    {
                        userModel.IsDeleted = true;
                        userModel.DeletedUserId = userId;
                        userModel.DeletedDate = DateTime.Now;
                        context.Users.Update(userModel);
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS);
                    }
                }
            }
            catch (Exception e)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = e.Message;
            }
            return response;
        }
        #endregion
        #region Login
        public ResponseModel Login(string email, string password)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var userData = (from user in context.Users
                                    where user.Email == email &&
                                          user.IsActive == true &&
                                          user.IsDeleted == false &&
                                          user.PasswordHash == password
                                    select new
                                    {
                                        user.Id,
                                        user.FirstName,
                                        user.IsActive
                                    }).FirstOrDefault();
                    if (userData != null)
                    {
                        response.Data = new Dictionary<string, string>
                        {
                            { "Id", userData.Id.ToString() },
                            { "FirstName", userData.FirstName },
                            { "IsActive", userData.IsActive.ToString() }
                        };
                    }
                    else
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = "User not found or login credentials are incorrect.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        #endregion

        #region EmailExits
        public ResponseModel EmailExists(string email)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var emailExist = context.Users.Any(u => u.Email == email);
                    if (emailExist != true)
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST, email);
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }
        #endregion
    }
}

				

