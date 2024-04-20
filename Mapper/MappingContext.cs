using AutoMapper;
using Reddit_App.Mapper;
using Reddit_App.Models;
using Reddit_App.Request;

namespace Reddit_App.Mapper
{
    public class MappingContext : Profile
    {
        public MappingContext()
        {
            CreateMap<CreateNewPost, Post>();
            CreateMap<UpdateUserInfoRequest, users>();
            CreateMap<UpdatePostRequest, Post>();
            CreateMap<LikeRequest, Like>();
            CreateMap<NewFollowRequest, Follow>();
        } 
    }
}
