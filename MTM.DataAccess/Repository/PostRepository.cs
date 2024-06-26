using MTM.DataAccess.IRepository;
using MTM.Entities.DTO;
using MTM.Entities.Data;

namespace MTM.DataAccess.Repository
{
    public class PostRepository : IPostRepository
    {
        #region GetUser
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
    }
}
