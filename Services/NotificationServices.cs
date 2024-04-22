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

        public async Task<MessageData> CreateNewNoti(int uID, string content)
        {
            try
            {
                var request = new CreateNewNotifications
                {
                    UserID = uID,
                    Content = content
                };
                var newNoti = _mapper.Map<Notifications>(request);
                _notirepo.Create(newNoti);
                _notirepo.SaveChange();
                return new MessageData { Data = newNoti, Des = "Add new notisuccess" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "Error" };
            }
        }
    }
}
