using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Request;
using Reddit_App.Services;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthenticateController : BaseApiController<UserAuthenticateController>
    {
        private readonly UserAuthenticateService _userAuthenticateService;
        
        public UserAuthenticateController(DatabaseContext dbcontext, ApiOptions apiCongig, IMapper mapper)
        {
            _userAuthenticateService = new UserAuthenticateService(apiCongig,dbcontext,mapper);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("UserLogin")]
        public MessageData UserLogin(LoginRequest request)
        {
            try
            {
                var res = _userAuthenticateService.UserLogin(request);
                return new MessageData { Data = res, Des = "Login success" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
