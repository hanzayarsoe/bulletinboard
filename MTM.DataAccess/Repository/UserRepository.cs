using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MTM.CommonLibrary;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        #region List
        /// <summary>
        ///  Get all data from category table
        /// </summary>
        /// <returns></returns>
        public UserListViewModel Data()
        {
            UserListViewModel list = new UserListViewModel();
            try
            {
                using (var context = new MTMContext())
                {
                    list.UserList = (from data in context.Users where data.IsActive == true & data.IsDeleted == false
                            select new UserViewModel {
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
        /// <summary>
        /// New category creation.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public ResponseModel Create(User user)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var checkExist = context.Users.FirstOrDefault(c => c.FirstName == user.FirstName);
                    if (checkExist != null)
                    {
                        response.ResponseType = Message.EXIST;
                        response.ResponseMessage = string.Format(Message.ALREADY_EXIST, user.FirstName);
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

        #region GetCategory
        /// <summary>
        /// To get a specific category data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserViewModel GetUser(string id)
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using(var context = new MTMContext())
                {
                    model = (from data in context.Users where 
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
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return model;
        }
        #endregion

        #region Update
        /// <summary>
        /// Update specific category data
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseModel Update(User user)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    User isExist = (from data in context.Users where 
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
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, user.FirstName,"updated");
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
        /// <summary>
        /// Delete specific data to update deleteFlag
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseModel Delete(string id, string userId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using(var context = new MTMContext())
                {
                    var userModel = context.Users.FirstOrDefault(c => c.Id == id);
                    if(userModel == null)
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

        #region EmailExits
        public ResponseModel EmailExists(string email)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var emailExist = context.Users.Any(u => u.Email == email);
                    if(emailExist != true)
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST,email);
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
