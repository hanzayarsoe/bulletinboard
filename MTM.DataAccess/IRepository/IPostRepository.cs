using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IPostRepository
    {
        //PostListViewModel Data();
        //PostViewModel Create(Post post);
        PostViewModel GetPost(string id);
        PostListViewModel GetPostList();
        ResponseModel Create(Post post);
        ResponseModel CreateList(List<Post> posts);
        ResponseModel Update(Post post);
        //PostViewModel Delete(string id, string userId);
        ResponseModel Delete(string id,string currentUserId);
    }
}
