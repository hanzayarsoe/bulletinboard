﻿using MTM.CommonLibrary;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using OfficeOpenXml;

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
        public UserListViewModel GetList(String LoginId)
        {

            UserListViewModel list = new UserListViewModel();
            try
            {
                using (var context = new MTMContext())
                {
                    list.UserList = (from user in context.Users
                                     join createdBy in context.Users 
                                     on user.CreatedUserId equals createdBy.Id
                                     where user.IsActive == true & user.IsDeleted == false
                                     & user.Id != LoginId

                                     select new UserViewModel
                                     {
                                         Id = user.Id,
                                         FullName = user.FirstName + " "+ user.LastName,
                                         Email = user.Email,
                                         Role = user.Role,
                                         DOB = user.Dob,
                                         Address = user.Address,
                                         PhoneNumber = user.PhoneNumber,
                                         RoleName = user.Role == 1 ? "admin" : "user",
                                         CreatedDate = user.CreatedDate,
                                         CreatedFullName = createdBy.FirstName + " "+createdBy.LastName,
                                         IsActive = user.IsActive ? true : false,
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
                             join createdBy in context.Users
                             on data.CreatedUserId equals createdBy.Id
                             where
                             data.Id == id &
                             data.IsActive == true &
                             data.IsDeleted == false
                             select new UserViewModel
                             {
                                 Id = data.Id,
                                 FirstName = data.FirstName,
                                 LastName = data.LastName,
                                 FullName = data.FirstName + " " + data.LastName,
                                 Address = data.Address,
                                 DOB = data.Dob,
                                 PhoneNumber = data.PhoneNumber,
                                 Role = data.Role,
                                 RoleName = data.Role == 1 ? "admin" : "user",
                                 Email = data.Email,
                                 IsActive = data.IsActive,
                                 CreatedDate = data.CreatedDate,
                                 CreatedFullName = createdBy.FirstName + " " + createdBy.LastName,

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

                    User? isExist = context.Users.FirstOrDefault(u => u.Id == user.Id);

                    if (isExist != null)
                    {
                        if (!string.IsNullOrEmpty(user.UserName))
                        {
                            isExist.UserName = user.UserName;
                            isExist.NormalizedUserName = user.UserName.ToUpper();
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            isExist.Email = user.Email;
                            isExist.NormalizedEmail = user.Email.ToUpper();
                        }
                        if (!string.IsNullOrEmpty(user.FirstName))
                        {
                            isExist.FirstName = user.FirstName;
                        }
                        if (!string.IsNullOrEmpty(user.LastName))
                        {
                            isExist.LastName = user.LastName;
                        }
                        if (!string.IsNullOrEmpty(user.PasswordHash))
                        {
                            isExist.PasswordHash = user.PasswordHash;
                        }
                        if (!string.IsNullOrEmpty(user.SecurityStamp))
                        {
                            isExist.SecurityStamp = user.SecurityStamp;
                        }
                        if (!string.IsNullOrEmpty(user.ConcurrencyStamp))
                        {
                            isExist.ConcurrencyStamp = user.ConcurrencyStamp;
                        }
                        if (!string.IsNullOrEmpty(user.PhoneNumber))
                        {
                            isExist.PhoneNumber = user.PhoneNumber;
                        }
                        if (!string.IsNullOrEmpty(user.Address))
                        {
                            isExist.Address = user.Address;
                        }
                        if (user.Dob.HasValue)
                        {
                            isExist.Dob = user.Dob;
                        }
                        if (user.Role.HasValue)
                        {
                            isExist.Role = user.Role.Value;
                        }

                        isExist.EmailConfirmed = user.EmailConfirmed;
                        isExist.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                        isExist.TwoFactorEnabled = user.TwoFactorEnabled;
                        isExist.LockoutEnd = user.LockoutEnd;
                        isExist.LoockoutEnabled = user.LoockoutEnabled;
                        isExist.AccessFailedCount = user.AccessFailedCount;
                        isExist.IsActive = user.IsActive;
                        isExist.IsDeleted = user.IsDeleted;
                        isExist.UpdatedDate = DateTime.Now;
                        isExist.UpdatedUserId = user.UpdatedUserId;

                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "your info", "updated");
                    }
                    else
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST,"your info");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = $"An error occurred while saving the entity changes: {innerException}";
            }
            catch (Exception ex)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = ex.Message;
            }

            return response;
        }
        #endregion

        #region Update Password 
        public ResponseModel UpdatePassword(string id, string oldPwd, string newPwd)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    User? isExist = context.Users.FirstOrDefault(u => u.Id == id);
                    if (!Helpers.VerifyPassword(oldPwd, isExist?.PasswordHash ?? String.Empty))
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = "Incorrect Old Password";
                        return response;
                    }

                    if (isExist != null)
                    {
                        isExist.PasswordHash = Helpers.HashPassword(newPwd);
                        isExist.UpdatedUserId = id;
                        isExist.UpdatedDate = DateTime.Now;
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "Password", "updated");
                    }
                    else
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST, "Your Account");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = $"An error occurred while saving the entity changes: {innerException}";
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
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "User" , "Deleted");
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

		#region Register
		public ResponseModel Register(User user)
		{
			ResponseModel response = new ResponseModel();
			try
			{
				using (var context = new MTMContext())
				{
					var checkExist = context.Users.FirstOrDefault(c => c.Email == user.Email);
					if (checkExist != null && checkExist.IsDeleted == false)
					{
						response.ResponseType = Message.EXIST;
						response.ResponseMessage = string.Format(Message.ALREADY_EXIST, user.Email);
					}
					else
					{
                        if(user.Role == null)
                        {
                            user.Role = context.Users.Count() == 0 ? 1 : 2;
                        }
                        user.PasswordHash = Helpers.HashPassword(user.PasswordHash);
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

		#region Login
		public ResponseModel Login(string email, string password)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var userData = (from user in context.Users
                                    where user.Email == email 
                                    select new
                                    {
                                        user.Id,
                                        user.Email,
                                        user.FirstName,
                                        user.LastName,
                                        user.PasswordHash,
                                        user.IsActive,
                                        user.IsDeleted,
                                        user.LoockoutEnabled
                                    }).FirstOrDefault();
                    if (userData != null)
                    {
                        if(!Helpers.VerifyPassword(password, userData.PasswordHash)){
							response.ResponseType = Message.FAILURE;
							response.ResponseMessage = "Incorrect Email or Password";
						}
                        else if(userData.IsDeleted == true)
                        {
                            response.ResponseType = Message.FAILURE;
                            response.ResponseMessage = "Your account was Deleted";
                        }else if(userData.IsActive == false)
                        {
							response.ResponseType = Message.FAILURE;
							response.ResponseMessage = "Your account was Deactivate";
						}
                        else if(userData.LoockoutEnabled == true)
                        {
                            response.ResponseType = Message.FAILURE;
                            response.ResponseMessage = "Your account was Locked";
                        }
                        else {
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

        #region GetIdByEmail
        public ResponseModel GetIdByEmail(string email)
        {
            var response = new ResponseModel();
            try
            {
                using var context = new MTMContext();

                var emailExist = context.Users
                    .Where(user => user.Email == email)
                    .Select(user => new
                    {
                        user.Id,
                        user.Email,
                        user.IsActive,
                        user.IsDeleted
                    })
                    .FirstOrDefault();

                if (emailExist == null)
                {
                    response.ResponseType = Message.FAILURE;
                    response.ResponseMessage = string.Format(Message.NOT_EXIST, email);
                }
                else if (!emailExist.IsDeleted || emailExist.IsActive)
                {
                    response.ResponseType = Message.SUCCESS;
                    response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "email", "sent to " + email);
                    response.Data = new Dictionary<string, string>
                    {
                        { "Id", emailExist.Id.ToString() },
                        { "Email", emailExist.Email }
                    };
                }
                else
                {
                    response.ResponseType = Message.FAILURE;
                    response.ResponseMessage = string.Format(Message.NOT_EXIST, email);
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

        #region CheckEmail
        public bool CheckEmail(string email)
        {
            bool isExist = false;

            if (email != null)
            {
                using var context = new MTMContext();
                isExist = context.Users.Any(user => user.Email == email);
                return isExist;
            }

            return isExist;
        }
        #endregion

        #region UploadUser
        public ResponseModel UploadUser(string filePath)
        {
            var response = new ResponseModel();
            var errorMessages = new List<string>();

            using(var context = new MTMContext())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var firstName = worksheet.Cells[row, 1].Text;
                        var lastName = worksheet.Cells[row, 2].Text;
                        var email = worksheet.Cells[row, 3].Text;
                        var phone = worksheet.Cells[row, 4].Text;
                        var password = worksheet.Cells[row, 5].Text;
                        var confirmPassword = worksheet.Cells[row, 6].Text;
                        var roleString = worksheet.Cells[row, 7].Text;
                        var dobString = worksheet.Cells[row, 8].Text;
                        var address = worksheet.Cells[row, 9].Text;

                        if(firstName == null)
                        {
                            errorMessages.Add($"First Name is required");
                            continue;
                        }

                        if (context.Users.Any(u => u.Email == email))
                        {
                            errorMessages.Add($"Email {email} already exists at row {row}");
                            continue;
                        }

                        if (password != confirmPassword)
                        {
                            errorMessages.Add($"Password and Confirm Password do not match at row {row}");
                            continue;
                        }

                        if (!int.TryParse(roleString, out int role))
                        {
                            errorMessages.Add($"Invalid role at row {row}");
                            continue;
                        }

                        if (!DateTime.TryParse(dobString, out DateTime dob))
                        {
                            errorMessages.Add($"Invalid date of birth at row {row}");
                            continue;
                        }

                        var newUser = new User
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            PhoneNumber = phone,
                            PasswordHash = Helpers.HashPassword(password),
                            Role = role,
                            Dob = dob,
                            Address = address
                        };

                        context.Users.Add(newUser);
                    }

                    if (errorMessages.Any())
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Join("; ", errorMessages);
                    }
                    else
                    {
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS,"User","Created");
                    }
                }
            }

            return response;
        }
        #endregion
    }
}


				
