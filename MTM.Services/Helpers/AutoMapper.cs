using AutoMapper;
using MTM.Entities.Data;
using MTM.Entities.DTO;

namespace MTM.Services.Helpers
{
   public class AutoMapperProfiles
    {
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<UserViewModel,User>().ReverseMap();
                CreateMap<PostViewModel,Post>().ReverseMap();
            }
        }
    }
}
