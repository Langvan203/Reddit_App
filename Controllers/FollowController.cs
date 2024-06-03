using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Request;
using Reddit_App.Services;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : BaseApiController<FollowController>
    {
        private readonly FollowServices _followServices;
        private readonly IMapper _mapper;
        public FollowController(DatabaseContext dbContext, ApiOptions apiOptions, IWebHostEnvironment webhost, IMapper mapper)
        {
            _mapper = mapper;
            _followServices = new FollowServices(apiOptions, mapper, dbContext, webhost);
        }

        [HttpGet]
        [Route("GetListFollow")]
        public MessageData GetAllFollow(int UserID)
        {
            try
            {
                var res = _followServices.GetAllFollow(UserID);
                return new MessageData { Data = res, Des = "get all follow success"};
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }


        [HttpPost]
        [Route("AddNewFollow")]

        public MessageData AddFollow(NewFollowRequest request)
        {
            try
            {
                var res = _followServices.AddNewFollow(UserIDLogined, request);
                return new MessageData { Data = res, Des = "Add new follow success" };
            }
            catch(Exception e)
            {
                return NG(e);
            }
        }

    }
}
