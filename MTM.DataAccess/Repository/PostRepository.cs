using MTM.DataAccess.IRepository;
using MTM.Entities.DTO;
using MTM.Entities.Data;
using Microsoft.EntityFrameworkCore;
using MTM.CommonLibrary;

namespace MTM.DataAccess.Repository
{
    public class PostRepository : IPostRepository
    {
        #region GetPost
        public PostViewModel GetPost(string id)
        {
            PostViewModel model = new PostViewModel();
            try
            {
                using (var context = new MTMContext())
                {
                    model = (from post in context.Posts
                             join createdBy in context.Users
                             on post.CreatedUserId equals createdBy.Id
                             where
                             post.Id == id 
                             select new PostViewModel
                             {
                                 Id = post.Id,
                                 Title = post.Title,
                                 Description = post.Description,
                                 IsPublished = post.IsPublished,
                                 CreatedUserId = post.CreatedUserId,
                                 CreatedDate = post.CreatedDate,
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

        #region GetPostList
        public PostListViewModel GetPostList()
        {

            PostListViewModel list = new PostListViewModel();
            try
            {
                using (var context = new MTMContext())
                {
                    list.PostList = (from post in context.Posts
                                     join createdBy in context.Users
                                     on post.CreatedUserId equals createdBy.Id
                                     where post.IsDeleted == false
                                     select new PostViewModel
                                     {
                                         Id = post.Id,
                                         Title = post.Title,
                                         Description = post.Description,
                                         IsPublished = post.IsPublished,
                                         CreatedDate = post.CreatedDate,
                                         CreatedFullName = createdBy.FirstName + " " + createdBy.LastName,

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
        public ResponseModel Create(Post post)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    context.Posts.Add(post);
                    context.SaveChanges();
                    response.ResponseType = Message.SUCCESS;
                    response.ResponseMessage = string.Format(Message.SAVE_SUCCESS,"Post", "created");
                }
            }
            catch (Exception ex)
            {
                response.ResponseType = Message.FAILURE;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        public ResponseModel CreateList(List<Post> posts)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (posts != null)
                {
                    using (var context = new MTMContext())
                    {
                        context.Posts.AddRange(posts);
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "Import", "completed");
                    }
                }
                else
                {
                    response.ResponseType = Message.FAILURE;
                    response.ResponseMessage = string.Format(Message.FAIL, "Import");
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

        #region Update
        public ResponseModel Update(Post post)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    Post? isExist = context.Posts.FirstOrDefault(p => p.Id == post.Id);
                    if(isExist != null)
                    {
                        isExist.Title = post.Title;
                        isExist.Description = post.Description;
                        isExist.IsPublished = post.IsPublished;
                        isExist.UpdatedDate = post.UpdatedDate;
                        isExist.UpdatedUserId = post.UpdatedUserId;
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "Post", "updated");
                    }
                    else
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST, "your info");
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
        public ResponseModel Delete(string id,string currentUserId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                using (var context = new MTMContext())
                {
                    var postModel = context.Posts.FirstOrDefault(c => c.Id == id);
                    if (postModel == null)
                    {
                        response.ResponseType = Message.FAILURE;
                        response.ResponseMessage = string.Format(Message.NOT_EXIST,"Post");
                    }
                    else
                    {
                        postModel.IsDeleted = true;
                        postModel.DeletedUserId = currentUserId;
                        postModel.DeletedDate = DateTime.Now;
                        postModel.IsPublished = false;
                        context.Posts.Update(postModel);
                        context.SaveChanges();
                        response.ResponseType = Message.SUCCESS;
                        response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "Post", "Deleted");
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
    }
}
