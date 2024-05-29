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
        public MessageData GetAllFollow()
        {
            try
            {
                var res = _followServices.GetAllFollow(UserIDLogined);
                return new MessageData { Data = res, Des = "get all follow success"};
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }


        [HttpGet]
        [Route("GetNumberFollow")]
        public MessageData GetNumberFollow()
        {
            try
            {
                var res = _followServices.GetNumberFollow(UserIDLogined);
                return new MessageData { Data = res.Data, Des = "get succesfull" };
            }
            catch (Exception ex)
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
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception e)
            {
                return NG(e);
            }
        }

        [HttpPost]
        [Route("Unfollow")]
        public MessageData Unfollow(NewFollowRequest request)
        {
            try
            {
                var res = _followServices.UnFollow(UserIDLogined, request);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
