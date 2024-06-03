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
            _commentservices = new CommentServices(options, dbContext, mapper, webHost);
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetListComment")]

        public MessageData GetListComment(int PostID)
        {
            try
            {
                var res = _commentservices.GetListComment(PostID);
                return new MessageData { Data = res, Des = "Get list comment succes" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }


        [HttpPost]
        [Route("AddNewComment")]
        public MessageData AddNewComment([FromForm] NewCommentRequest request)
        {
            try
            {
                var res = _commentservices.AddNewComment(UserIDLogined, request);
                return new MessageData { Data = res, Des = "add new comment succes" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpPut]
        [Route("UpdateComment")]
        public MessageData UpdateComment([FromForm] UpdateCommentRequest request)
        {
            try
            {
                var res = _commentservices.UpdateComment(UserIDLogined, request);
                return new MessageData { Data = res, Des = "Update comment success" };

            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpDelete]
        [Route("DeleteCommentByUser")]

        public MessageData DeleteCommentByUser(int CommentID)
        {
            try
            {
                var res = _commentservices.DeleteComment(UserIDLogined, CommentID);
                return new MessageData { Data = res, Des = "Delete comment succes" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
