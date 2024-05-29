using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;
using System.Net.WebSockets;

namespace Reddit_App.Services
{
    public class FollowServices
    {
        private readonly ApiOptions _apiOption;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHost;
        private readonly FollowRepository _followRespository;
        private readonly usersRepository _userRepository;
        public FollowServices(ApiOptions apiOptions, IMapper mapper, DatabaseContext dbContext, IWebHostEnvironment webHost)
        {
            _followRespository = new FollowRepository(apiOptions, dbContext, mapper);
            _userRepository = new usersRepository(apiOptions, dbContext, mapper);
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
        public object GetAllFollow(int UserLoginedID)
        {
            try
            {
                // this code bellow is n+1 query problem?
                // was fix N+1 Query problem
                var res = _followRespository.FindAll().Where(f => f.FollowerID == UserLoginedID && f.Status == 1).ToList();
                var followedIDs = res.Select(r => r.FollowedID).ToList();
                var users = _userRepository.FindByCondition(p => followedIDs.Contains(p.UserID)).ToList();
                List<ListFollowDto> listFollow = new List<ListFollowDto>();
                foreach(var item in res)
                {
                    var followDto = new ListFollowDto();
                    followDto.UserID = item.FollowedID;
                    var checkUsername = users.FirstOrDefault(p => p.UserID == item.FollowedID);
                    if(checkUsername != null)
                    {
                        followDto.UserName = checkUsername.UserName;
                        followDto.Avata = checkUsername.Image;
                    }
                    else
                    {
                        followDto.UserName = "";
                        followDto.Avata = "";
                    }
                     
                    listFollow.Add(followDto);
                }
                listFollow.Reverse();
                var ListFollowUser = new
                {
                    Following = listFollow,
                    NumberFollow = listFollow.Count()
                };
                    
                return ListFollowUser;
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
