﻿using MTM.CommonLibrary;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using System;

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
                                 LastName = data.LastName,
                                 Email = data.Email,
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
                    User isExist = context.Users.FirstOrDefault(u => u.Id == user.Id);

                    if (isExist != null)
                    {
                        isExist.PasswordHash = user.PasswordHash; // Update only relevant fields
                        context.Users.Update(isExist);
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS,"your password", "updated");
                    }
                    else
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = "User does not exist.";
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
                                          user.PasswordHash == password
                                    select new
                                    {
                                        user.Id,
                                        user.Email,
                                        user.FirstName,
                                        user.LastName,
                                        user.IsActive,
                                        user.IsDeleted
                                    }).FirstOrDefault();
                    if (userData != null)
                    {
                        if(userData.IsDeleted == true || userData.IsActive == false)
                        {
                            response.ResponseType = Message.FAILURE;
                            response.ResponseMessage = "User was not activate";
                        }
                        else
                        {
							response.ResponseType = Message.SUCCESS;
							response.Data = new Dictionary<string, string>
						    {
							    { "Id", userData.Id.ToString() },
							    { "Email", userData.Email },
                                { "FullName", userData.FirstName + " " + userData.LastName },
						    };
						}
                    }
                    else
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = "Incorrect Email or Password";
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
                    var emailExist = (from user in context.Users 
                                      where user.Email == email
                                      select new
                                      {
                                          user.Id,
                                          user.Email,
                                          user.IsActive,
                                          user.IsDeleted
                                      }).FirstOrDefault() ;
                    if (emailExist == null)
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST, email);
                    }
                    else
                    {
                        if(emailExist.IsDeleted != true || emailExist.IsActive == true)
                        {
                            response.ResponseType = Message.SUCCESS;
                            response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "email", "sent to" + email);
                            response.Data = new Dictionary<string, string>
                            {
                                { "Id", emailExist.Id.ToString() },
                                { "Email", emailExist.Email },
                            };
                        }
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

				

