using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IPostService
    {
        PostViewModel GetPost(string id);
        PostListViewModel GetPostList(UserViewModel user);
        ResponseModel Update(PostViewModel post);
        ResponseModel Create(PostViewModel model);
        ResponseModel Delete(string id,string currentUserId);
    }
}
