using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Request;
using Reddit_App.Services;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseApiController<NotificationController>
    {
        private readonly NotificationServices _notiServices;
        private readonly IMapper _mapper;
        
        private readonly HttpClient _httpClient;
        public NotificationController(DatabaseContext dbcontext, IMapper mapper, IWebHostEnvironment webHost, ApiOptions apiOptions)
        {
            _mapper = mapper;
            _notiServices = new NotificationServices(apiOptions, mapper, webHost, dbcontext);
          
        }

        [HttpPost]
        [Route("CreateNewNoti")]

        public MessageData CreateNewNoti(CreateNewNotificationsRequest request)
        {
            try
            {
                var res = _notiServices.CreateNewNoti(UserIDLogined, request);
                return new MessageData { Data = res, Des = "send notification successfull" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetListNotification")]
        public MessageData GetListNoti()
        {
            try
            {
                var res = _notiServices.GetNotification(UserIDLogined);
                return new MessageData { Data = res, Des = "Get noti" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
