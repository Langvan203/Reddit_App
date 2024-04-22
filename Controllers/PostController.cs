using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Request;
using Reddit_App.Services;
using SignalRChat.Hubs;
 
namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : BaseApiController<PostController>
    {
        private readonly PostServices _postServices;
        private readonly IMapper _mapper;
        private readonly IHubContext<DetectNewEvent> _hubContext;
        //private readonly HttpClient _httpClient;
        public PostController(DatabaseContext dbcontext, IMapper mapper, IWebHostEnvironment webHost, ApiOptions apiOptions, IHubContext<DetectNewEvent> hubContext)
        {
            _mapper = mapper;
            _hubContext = hubContext;
            _postServices = new PostServices(apiOptions, dbcontext, mapper, webHost);
            //_httpClient = httpClient;
        }

         //Create new post 
        [HttpPost]
        [Route("CreateNewPost")]
        public MessageData AddNewPosts([FromForm] CreateNewPost request)
        {
            try
            {
                var res = _postServices.AddNewPost(request, UserIDLogined);
                //await _hubContext.Clients.All.SendAsync("ReceiveNotification", "A new post has been added.");
                _hubContext.Clients.All.SendAsync("ReceiveNotification", "Có bài viết mới được tạo.");
                //var response = await _httpClient.PostAsync("https://localhost:7036/api/Notification/CreateNewNoti", null);
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
