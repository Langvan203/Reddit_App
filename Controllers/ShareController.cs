using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reddit_App.Services;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShareController : BaseApiController<ShareController>
    {
        private readonly IMapper _mapper;
        private readonly ShareServices _shareServices;

    }
}
