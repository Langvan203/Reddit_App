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
                var newFollow = _mapper.Map<Follow>(request);
                _followRespository.Create(newFollow);
                _followRespository.SaveChange();
                return new MessageData { Data = newFollow , Des = "follow success"};
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
                var res = _followRespository.FindAll().Where(f => f.FollowerID == UserID_er);
                return new MessageData { Data = res, Des = "Get list follow succes" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "get list failed" };
            }
        }

        public MessageData UnFollow(int UserID_er, int UserID_ed)
        {
            // UserID_er là người được follow
            // UserID_ed là người thực hiện follow
            try
            {
                var res = _followRespository.FindByCondition(u => u.FollowerID == UserID_er && u.FollowedID == UserID_ed).FirstOrDefault();
                if(res == null)
                {
                    return new MessageData { Data = null, Des = "Error" };
                }    
                else
                {
                    _followRespository.DeleteByEntity(res);
                    _followRespository.SaveChange();
                    return new MessageData { Data = res, Des = "Unfollow success" };
                }    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // will fix
        public MessageData GetNumberFollow(int userID_er)
        {
            try
            {
                var numberFollower = _followRespository.FindByCondition(c => c.FollowerID == userID_er).GroupBy(c => c.FollowerID).Select(g => new
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
