using AutoMapper;
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
    public class CommentController : BaseApiController<CommentController>
    {
        private readonly CommentServices _commentservices;
        private readonly IMapper _mapper;

        public CommentController(DatabaseContext dbContext, ApiOptions options, IWebHostEnvironment webHost, IMapper mapper)
        {
            _commentservices = new CommentServices(options,dbContext, mapper, webHost);
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetListComment")]

        public MessageData GetListComment(int PostID)
        {
            try
            {
                var res = _commentservices.GetListComment(PostID);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetTotalComment")]
        public MessageData GetToTalComment(int PostID)
        {
            try
            {
                var res = _commentservices.GetTotalComment(PostID);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpPost]
        [Route("AddNewComment")]
        public MessageData AddNewComment(NewCommentRequest request)
        {
            try
            {
                var res = _commentservices.AddNewComment(UserIDLogined, request);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
