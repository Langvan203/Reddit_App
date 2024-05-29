using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Request;
using Reddit_App.Services;
using SQLitePCL;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShareController : BaseApiController<ShareController>
    {
        private readonly IMapper _mapper;
        private readonly ShareServices _shareServices;
        public ShareController(ApiOptions apiOptions, DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHost)
        {
            _shareServices = new ShareServices(apiOptions, dbContext, mapper, webHost);
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetListShare")]
        public MessageData GetListShare(int PostID)
        {
            try
            {
                var res = _shareServices.GetListShare(PostID);
                return new MessageData { Data = res, Des = "Get list share succes" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpPost]
        [Route("Add new share")]
        public MessageData AddNewShare(ShareRequest request)
        {
            try
            {
                var res = _shareServices.AddNewShare(UserIDLogined, request);
                return new MessageData { Data = res, Des = "add new share success" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetTotalShare")]
        public MessageData GetTotalShare(int PostID)
        {
            try
            {
                var res = _shareServices.GetNumberShare(PostID);
                return new MessageData { Data = res, Des = "Get total share success" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
