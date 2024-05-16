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

        public MessageData AddNewShare(int UserLogined,ShareRequest request)
        {
            try
            {
                var newShare = _shareRepository.FindByCondition(s => s.PostID == request.PostID && s.ShareStatus == 1 && s.UserID == UserLogined).FirstOrDefault();
                if (newShare == null)
                {
                    var nShare = _mapper.Map<Share>(request);
                    nShare.UserID = UserLogined;
                    nShare.ShareStatus = 1;
                    _shareRepository.Create(nShare);
                    _shareRepository.SaveChange();
                    return new MessageData { Data = nShare, Des = "Add new share success" };
                }
                return new MessageData { Data = null, Des = "Share is already exsist" };
            }
            catch (Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        public MessageData GetListShare(int PostID)
        {
            try
            {
                var lShare = _shareRepository.FindByCondition(s => s.PostID == PostID);
                return new MessageData { Data = lShare, Des = "Get list user share success" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        public MessageData UnShare(int UserLogined, ShareRequest request)
        {
            try
            {
                var UShare = _shareRepository.FindByCondition(s => s.UserID == UserLogined && s.PostID == request.PostID && s.ShareStatus == 1).FirstOrDefault();
                if(UShare != null)
                {
                    UShare.ShareStatus = 0;
                    _shareRepository.UpdateByEntity(UShare);
                    _shareRepository.SaveChange();
                    return new MessageData { Data = UShare, Des = "Un share success" };
                }
                else
                {
                    return new MessageData { Data = null, Des = "error when un share" };
                }
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        public MessageData GetNumberShare(int PostID)
        {
            try
            {
                var nShare = _shareRepository.FindByCondition(s => s.PostID == PostID && s.ShareStatus == 1).GroupBy(l => l.PostID).Select(l => new
                {
                    PostID = l.Key,
                    NumberShare = l.Count()
                });
                return new MessageData { Data = nShare, Des = "Get total number share success" };
            }
            catch(Exception ex) {

                return new MessageData { Data = null, Des = ex.Message };
            }
        }
    }
}
