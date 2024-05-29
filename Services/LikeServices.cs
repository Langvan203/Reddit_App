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
        private readonly usersRepository _userRepository;
        public LikeServices(ApiOptions apiOptions, IMapper mapper, IWebHostEnvironment webHost, DatabaseContext dbContext)
        {
            _likeRepository = new LikeRepository(apiOptions, dbContext, mapper);
            _userRepository = new usersRepository(apiOptions, dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper;
            _webHost = webHost;
        }

        public object AddNewLike(LikeRequest request, int UserID)
        {
            try
            {
                var newLike = _likeRepository.FindByCondition(l => l.PostID == request.PostID && l.UserID == UserID).FirstOrDefault();
                if(newLike == null)
                {
                    var nlike = _mapper.Map<Like>(request);
                    nlike.UserID = UserID;
                    nlike.LikeStatus = 1;
                    _likeRepository.Create(nlike);
                    _likeRepository.SaveChange();
                    return nlike;
                }    
                else
                {
                    if(newLike.LikeStatus == 0)
                    {
                        newLike.LikeStatus = 1;
                    }    
                    else if(newLike.LikeStatus == 1)
                    {
                        newLike.LikeStatus = 0;
                    }
                    _likeRepository.UpdateByEntity(newLike);
                    _likeRepository.SaveChange();
                    return newLike;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
 

        public object getListLike(int PostID)
        {
            try
            {
                var res = _likeRepository.FindByCondition(l => l.PostID == PostID).ToList();
                var listUserID = res.Select(p => p.UserID);
                var users = _userRepository.FindByCondition(p => listUserID.Contains(p.UserID)).ToList();
                List<GetLikeDto> listLike = new List<GetLikeDto>();
                foreach(var item in res)
                {
                    var likedto = new GetLikeDto();
                    var checkUserInfor = users.FirstOrDefault(p => p.UserID == item.UserID);
                    if(checkUserInfor != null)
                    {
                        likedto.UserID = checkUserInfor.UserID;
                        likedto.UserName = checkUserInfor.UserName;
                        likedto.Avata = checkUserInfor.Image;
                    }
                    listLike.Add(likedto);
                }
                listLike.Reverse();
                return new
                {
                    ListLike = listLike,
                    NumberLike = listLike.Count()
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
