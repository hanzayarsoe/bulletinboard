using AutoMapper;
using MTM.DataAccess.IRepository;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using MTM.Services.IService;

namespace MTM.Services.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            this._postRepository = postRepository;
            this._mapper = mapper;
        }
        public PostViewModel GetPost(string id)
        {
            return this._postRepository.GetPost(id);
        }
        public ResponseModel Update(PostViewModel model)
        {
            return this._postRepository.Update(this._mapper.Map<Post>(model));
        }

    }
}
