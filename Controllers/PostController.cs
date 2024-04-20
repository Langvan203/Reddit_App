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
    public class PostController : BaseApiController<PostController>
    {
        private readonly PostServices _postServices;
        private readonly IMapper _mapper;
        public PostController(DatabaseContext dbcontext, IMapper mapper, IWebHostEnvironment webHost, ApiOptions apiOptions)
        {
            _mapper = mapper;
            _postServices = new PostServices(apiOptions, dbcontext, mapper, webHost);
        }

        //Create new post 
        [HttpPost]
        [Route("CreateNewPost")]
        public MessageData AddNewPosts([FromForm] CreateNewPost request)
        {
            try
            {
                var res = _postServices.AddNewPost(request, UserIDLogined);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        // get list post
        [HttpGet]
        [Route("GetListPost")]
        public MessageData GetAllPost()
        {
            try
            {
                var res = _postServices.GetAllPost();
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetPostByTag")]
        public MessageData GetPostByTag(int tagID)
        {
            try
            {
                var res = _postServices.GetPostByTag(tagID);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpGet]
        [Route("GetPostByUserID")]
        public MessageData GetPostByUserID()
        {
            try
            {
                var res = _postServices.GetPostByUser(UserIDLogined);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpPut]
        [Route("UpdatePost")]
        [Authorize(Roles ="Admin")]
        public MessageData UpdatePost([FromForm] UpdatePostRequest request)
        {
            try
            {
                var res = _postServices.UpdatePost(request, UserIDLogined);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        
    }
}
