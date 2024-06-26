using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.DataAccess.IRepository
{
    public interface IPostRepository
    {
        //PostListViewModel Data();
        //PostViewModel Create(Post post);
        PostViewModel GetPost(string id);
        ResponseModel Update(Post post);
        //PostViewModel Delete(string id, string userId);
    }
}
