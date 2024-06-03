using AutoMapper;
using NetTopologySuite.Index.Strtree;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;

namespace Reddit_App.Services
{
    public class ShareServices
    {
        private readonly ShareRepository _shareRepository;
        private readonly usersRepository _userRepository;
        private readonly ApiOptions _apiOptions;
        private IWebHostEnvironment _webhost;
        private readonly IMapper _mapper;
        
        public ShareServices(ApiOptions apiOptions, DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webhost)
        {
            _shareRepository = new ShareRepository(apiOptions, dbContext, mapper);
            _userRepository = new usersRepository(apiOptions, dbContext, mapper);
            _mapper = mapper;
            _apiOptions = apiOptions;
            _webhost = webhost;
        }

        public object AddNewShare(int UserLogined,ShareRequest request)
        {
            try
            {
                var newShare = _shareRepository.FindByCondition(s => s.PostID == request.PostID && s.UserID == UserLogined).FirstOrDefault();
                if (newShare == null)
                {
                    var nShare = _mapper.Map<Share>(request);
                    nShare.UserID = UserLogined;
                    nShare.ShareStatus = 1;
                    _shareRepository.Create(nShare);
                    _shareRepository.SaveChange();
                    return nShare;
                }
                else
                {
                    if (newShare.ShareStatus == 1)
                    {
                        newShare.ShareStatus = 0;
                    }
                    else
                    {
                        newShare.ShareStatus = 1;
                    }
                    _shareRepository.UpdateByEntity(newShare);
                    _shareRepository.SaveChange();
                    return null;
                }    
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetListShare(int UserLoginedID)
        {
            try
            {
                var lShare = _shareRepository.FindByCondition(s => s.UserID == UserLoginedID).ToList();
                var lUser = lShare.Select(s => s.UserID).ToList();
                var users = _userRepository.FindByCondition(u => lUser.Contains(u.UserID)).ToList();
                List<GetListShareDto> listShare = new List<GetListShareDto>();
                foreach(var item in lShare)
                {
                    var shareU = new GetListShareDto();
                    
                    var checkUser = users.FirstOrDefault(u => u.UserID == item.UserID);
                    if(checkUser != null)
                    {
                        shareU.UserName = checkUser.UserName;
                        shareU.UserID = checkUser.UserID;
                        shareU.Image = checkUser.Image;
                        
                    }
                    shareU.PostID = item.PostID;
                    listShare.Add(shareU);
                }
                listShare.Reverse();
                return listShare;
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        
    }
}
