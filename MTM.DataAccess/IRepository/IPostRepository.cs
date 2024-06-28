using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IPostRepository
    {
        PostViewModel GetPost(string id);
        PostListViewModel GetPostList(User user);
        ResponseModel Create(Post post);
        ResponseModel CreateList(List<Post> posts);
        ResponseModel Update(Post post);
        ResponseModel Delete(string id,string currentUserId);
    }
}
