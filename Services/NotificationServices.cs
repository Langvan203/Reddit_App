using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;

namespace Reddit_App.Services
{
    public class NotificationServices
    {
        private readonly ApiOptions _apiOption;
        private readonly IMapper _mapper;
        private readonly NotificationRepository _notirepo;
        private readonly IWebHostEnvironment _webhost;
        
        public NotificationServices(ApiOptions apiOptions, IMapper mapper, IWebHostEnvironment webHost, DatabaseContext dbContext)
        {
            _notirepo = new NotificationRepository(apiOptions, dbContext, mapper);
            _mapper = mapper;
            _apiOption = apiOptions;
            _webhost = webHost;
        }

        public object CreateNewNoti(int SenderID, CreateNewNotificationsRequest request)
        {
            try
            {
                var p = _mapper.Map<Notifications>(request);
                p.SenderID = SenderID;
                _notirepo.Create(p);
                _notirepo.SaveChange();
                return p;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object GetNotification(int UserLoginedID)
        {
            try
            {
                var res = _notirepo.FindByCondition(u => u.ReceiverID == UserLoginedID);
                if(res != null)
                {
                    return res;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
