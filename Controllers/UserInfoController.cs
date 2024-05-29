using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reddit_App.Database;
using Reddit_App.Services;
using System.Runtime.CompilerServices;
using Reddit_App.Common;
using Reddit_App.Dto;
using Reddit_App.Request;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : BaseApiController<UserInfoController>
    {
        private readonly UserInfoServices _userinfo;
        private readonly IMapper _mapper;
        public UserInfoController(ApiOptions apiOptions, DatabaseContext Context, IWebHostEnvironment webHost, IMapper mapper)
        {
            _mapper = mapper;
            _userinfo = new UserInfoServices(apiOptions, Context, webHost, mapper);
        }

        [HttpGet]
        [Route("GetUserInfor")]
        public MessageData GetUserInfor()
        {
            try
            {
                var res = _userinfo.GetInfo(UserIDLogined);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        public MessageData UpdateUserInfor(UpdateUserInfoRequest request)
        {
            try
            {
                var res = _userinfo.UpdateInfo(request, UserIDLogined);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
