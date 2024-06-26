using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IPostService
    {
        //PostListViewModel Data();
        //PostViewModel Create(PostViewModel model);
        PostViewModel GetPost(string id);
        //PostViewModel Update(PostViewModel category);
        //PostViewModel Delete(string id, string userId);
    }
}
