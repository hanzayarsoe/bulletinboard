﻿using MTM.Entities.DTO;

namespace MTM.Services.IService
{
    public interface IPostService
    {
        //PostListViewModel Data();
        //PostViewModel Create(PostViewModel model);
        PostViewModel GetPost(string id);
        PostListViewModel GetPostList(UserViewModel user);
        ResponseModel Update(PostViewModel post);
        //PostViewModel Delete(string id, string userId);
        ResponseModel Create(PostViewModel model);
        ResponseModel Delete(string id,string currentUserId);
    }
}
