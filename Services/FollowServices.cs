using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;

namespace Reddit_App.Services
{
    public class FollowServices
    {
        private readonly ApiOptions _apiOption;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHost;
        private readonly FollowRepository _followRespository;
        public FollowServices(ApiOptions apiOptions, IMapper mapper, DatabaseContext dbContext, IWebHostEnvironment webHost)
        {
            _followRespository = new FollowRepository(apiOptions, dbContext, mapper);
            _apiOption = apiOptions;
            _mapper = mapper;
            _webHost = webHost;
        }

        public MessageData AddNewFollow(int userID_er, NewFollowRequest request)
        {
            // userID_er là người thực hiện follow;
            //request là người được chọn follow;
            try
            {
                var res = _followRespository.FindByCondition(f => f.FollowerID == userID_er && f.FollowedID == request.FollowedID).FirstOrDefault();
                if(res == null)
                {
                    var newFollow = _mapper.Map<Follow>(request);
                    newFollow.FollowerID = userID_er;
                    _followRespository.Create(newFollow);
                    _followRespository.SaveChange();
                    return new MessageData { Data = newFollow, Des = "following success" };
                }    
                else
                {
                    return new MessageData { Data = null, Des = "you has been Followed this user" };
                }    
            }
            catch
            {
                return new MessageData { Data = null, Des = "Error" };
            }
        }
        public MessageData GetAllFollow(int userID_er)
        {
            try
            {
                var res = _followRespository.FindAll().Where(f => f.FollowerID == userID_er);
                return new MessageData { Data = res, Des = "Get list follow succes" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "get list failed" };
            }
        }


        // will fix
        public async Task<object> GetNumberFollow(int userID_er)
        {
            try
            {
                var numberFollower = _followRespository.CountByConditionAsync(c => c.FollowerID == userID_er);
                return numberFollower;
            }
            catch
            {
                return null;
            }
        }
    }
}
