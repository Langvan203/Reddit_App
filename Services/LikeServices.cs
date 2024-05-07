using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;

namespace Reddit_App.Services
{
    public class LikeServices
    {
        private readonly ApiOptions _apiOptions;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHost;
        private readonly LikeRepository _likeRepository;
        public LikeServices(ApiOptions apiOptions, IMapper mapper, IWebHostEnvironment webHost, DatabaseContext dbContext)
        {
            _likeRepository = new LikeRepository(apiOptions, dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper;
            _webHost = webHost;
        }

        public MessageData AddNewLike(LikeRequest request, int UserID)
        {
            try
            {
                var newLike = _likeRepository.FindByCondition(l => l.PostID == request.PostID && l.UserID == UserID && l.LikeStatus == 1).FirstOrDefault();
                if(newLike == null)
                {
                    var nlike = _mapper.Map<Like>(request);
                    nlike.UserID = UserID;
                    nlike.LikeStatus = 1;
                    _likeRepository.Create(nlike);
                    _likeRepository.SaveChange();
                    return new MessageData { Data = nlike, Des = "Like success" };
                }         
                return new MessageData { Data = null, Des = "Like already exists"};
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "Error" };
            }

        }

        public MessageData DisLike(LikeRequest request, int UserID)
        {
            try
            {
                var res = _likeRepository.FindAll().Where(p => p.PostID == request.PostID && p.UserID == UserID && p.LikeStatus == 1).FirstOrDefault();
                if(res == null)
                {
                    return new MessageData { Data = null, Des ="Unlike error" }; 
                }    
                else
                {
                    res.LikeStatus = 0;
                    _likeRepository.UpdateByEntity(res);
                    _likeRepository.SaveChange();
                    return new MessageData { Data = res, Des = "Unlike success" };
                }    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public MessageData getListLike(LikeRequest request)
        {
            try
            {
                var res = _likeRepository.FindAll().Where(l => l.PostID == request.PostID && l.LikeStatus == 1);
                return new MessageData { Data = res, Des = "get list like success" };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public MessageData getNumberLikePost(LikeRequest request)
        {
            try
            {
                var res = _likeRepository.FindByCondition(l => l.PostID == request.PostID).GroupBy(l => l.PostID).Select(l => new
                {
                    PostID = l.Key,
                    LikeNumber = l.Count()
                });
                return new MessageData { Data = res, Des = "Get number like post success" };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


    }
}
