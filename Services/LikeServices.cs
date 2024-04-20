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
                var newLike = _mapper.Map<Like>(request);
                _likeRepository.Create(newLike);
                _likeRepository.SaveChange();
                return new MessageData { Data =newLike, Des = "Like post success"};
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
                var res = _likeRepository.FindAll().Where(p => p.PostID == request.PostID && p.UserID == UserID).FirstOrDefault();
                if(res == null)
                {
                    return new MessageData { Data = null, Des ="Unlike error" }; 
                }    
                else
                {
                    _likeRepository.DeleteByEntity(res);
                    _likeRepository.SaveChange();
                    return new MessageData { Data = res, Des = "Unlike success" };
                }    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
