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

        public object AddNewFollow(int userID_er, NewFollowRequest request)
        {
            try
            {
                var temp = _followRespository.FindByCondition(e => e.FollowerID == userID_er && e.FollowedID == request.FollowedID).FirstOrDefault();
                if(temp == null)
                {
                    var newFollow = _mapper.Map<Follow>(request);
                    newFollow.FollowerID = userID_er;
                    newFollow.Status = 1;
                    _followRespository.Create(newFollow);
                    _followRespository.SaveChange();
                    return newFollow;
                }
                else
                {
                    if(temp.Status == 0)
                    {
                        temp.Status = 1;
                    }
                    else
                    {
                        temp.Status = 0;
                    }
                    _followRespository.UpdateByEntity(temp);
                    _followRespository.SaveChange();
                    return temp;
                }
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public object GetAllFollow(int UserLoginedID)
        {
            try
            {
                // was fix N+1 Query problem
                var res = _followRespository.FindByCondition(f => f.FollowerID == UserLoginedID && f.Status == 1).ToList();
                var resFollowed = _followRespository.FindByCondition(u => u.FollowedID == UserLoginedID && u.Status == 1).ToList();
                var followedIDs = res.Select(r => r.FollowedID).ToList();
                var followerID = resFollowed.Select(p => p.FollowerID).ToList();
                var users = _userRepository.FindByCondition(p => followedIDs.Contains(p.UserID)).ToList();
                var usersFollowed = _userRepository.FindByCondition(p => followerID.Contains(p.UserID)).ToList();
                List<ListFollowDto> listFollow = new List<ListFollowDto>();
                List<ListFollowDto> listFollowed = new List<ListFollowDto>();
                foreach (var item in res)
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
                foreach (var item in resFollowed)
                {
                    var followDto = new ListFollowDto();
                    followDto.UserID = item.FollowerID;
                    var checkUsername = usersFollowed.FirstOrDefault(p => p.UserID == item.FollowerID);
                    if (checkUsername != null)
                    {
                        followDto.UserName = checkUsername.UserName;
                        followDto.Avata = checkUsername.Image;
                    }
                    else
                    {
                        followDto.UserName = "";
                        followDto.Avata = "";
                    }

                    listFollowed.Add(followDto);
                }
                listFollow.Reverse();
                listFollowed.Reverse();
                var ListFollowUser = new
                {
                    DanhSachNguoiBanTheoDoi = listFollow,
                    DanhSachNguoiTheoDoiBan = listFollowed,
                    SoLuongNguoiBanTheoDoi = listFollow.Count(),
                    SoLuongNguoiTheoDoiBan = listFollowed.Count()
                };
                    
                return ListFollowUser;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


    }
}
