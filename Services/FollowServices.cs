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
            try
            {
                
                var temp = _followRespository.FindByCondition(e => e.FollowerID == userID_er && e.FollowedID == request.FollowedID && e.Status == 1).FirstOrDefault();
                if(temp == null)
                {
                    var newFollow = _mapper.Map<Follow>(request);
                    newFollow.FollowerID = userID_er;
                    newFollow.Status = 1;
                    _followRespository.Create(newFollow);
                    _followRespository.SaveChange();
                    return new MessageData { Data = newFollow, Des = "follow success" };
                }
                return new MessageData { Data = null, Des = "Da thuc hien follow nguoi nay" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "Error" };
            }
        }
        public MessageData GetAllFollow(int UserID_er)
        {
            try
            {
                var res = _followRespository.FindAll().Where(f => f.FollowerID == UserID_er && f.Status == 1);
                return new MessageData { Data = res, Des = "Get list follow succes" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "get list failed" };
            }
        }

        public MessageData UnFollow(int UserID_er, NewFollowRequest request)
        {
            try
            {
                var res = _followRespository.FindByCondition(u => u.FollowerID == UserID_er && u.FollowedID == request.FollowedID && u.Status == 1).FirstOrDefault();
                if(res == null)
                {
                    return new MessageData { Data = null, Des = "Error" };
                }    
                else
                {
                    res.Status = 0;
                    _followRespository.UpdateByEntity(res);
                    _followRespository.SaveChange();
                    return new MessageData { Data = res, Des = "Unfollow success" };
                }    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public MessageData GetNumberFollow(int userID_er)
        {
            try
            {
                var numberFollower = _followRespository.FindByCondition(c => c.FollowerID == userID_er && c.Status == 1).GroupBy(c => c.FollowerID).Select(g => new
                {
                    IDNguoiDuocFollow = g.Key,
                    SoLuongFollow = g.Count()
                }).ToList();
                return new MessageData { Data = numberFollower, Des = "Get number follow success" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
