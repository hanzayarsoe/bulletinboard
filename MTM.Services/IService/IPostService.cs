using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IPostService
    {
        //PostListViewModel Data();
        //PostViewModel Create(PostViewModel model);
        PostViewModel GetPost(string id);
        ResponseModel Update(PostViewModel post);
        //PostViewModel Delete(string id, string userId);
    }
}
