﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Helpers.SendnotificationHub;
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
        //private readonly HttpClient _httpClient;
        public PostController(DatabaseContext dbcontext, IMapper mapper, IWebHostEnvironment webHost, ApiOptions apiOptions, IHubContext<NotificationHub> hubContext)
        {
            _mapper = mapper;
            _postServices = new PostServices(apiOptions, dbcontext, mapper, webHost, hubContext);
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
                return new MessageData { Data = res, Des = "Add new post success" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        //get list post
       [HttpGet]
       [Route("GetListPost")]
        public MessageData GetAllPost()
        {
            try
            {
                var res = _postServices.GetListPost();
                return new MessageData { Data = res, Des = "Get post" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetPostByID")]
        public MessageData GetPostByID(int PostID)
        {
            try
            {
                var res = _postServices.GetPostByID(PostID);
                return new MessageData { Data = res, Des = "Get post" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetPostByTag")]
        public MessageData GetPostByTag(int TagID)
        {
            try
            {
                var res = _postServices.GetPostByTag(TagID);
                return new MessageData { Data = res, Des = "Get post" };
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
                return new MessageData { Data = res, Des = "Get post by user success" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpPut]
        [Route("UpdatePost")]
        public MessageData UpdatePost([FromForm] UpdatePostRequest request)
        {
            try
            {
                var res = _postServices.UpdatePost(request, UserIDLogined);
                return new MessageData { Data = res, Des = "Update post success" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpDelete]
        [Route("DeletePost")]
        public MessageData DeletPost(int postID)
        {
            try
            {
                var res = _postServices.DeletePostByID(postID, UserIDLogined);
                return new MessageData { Data = res, Des = "Delete post success" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpGet]
        [Route("GetPostContent")]
        public MessageData GetPostByContentAndTitle(string content)
        {
            try
            {
                var res = _postServices.GetPostByContent(content);
                return new MessageData { Data = res, Des = "Get post by content succes" };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpGet]
        [Route("GetListPostAdmin")]
        [Authorize(Roles = "Admin")]
        public MessageData GetPostByAdmin()
        {
            try
            {
                var res = _postServices.GetListPostByAdmin();
                return new MessageData { Data = res, Des = "Get post by content succes" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpDelete]
        [Route("DeletePostByAdmin")]
        [Authorize(Roles = "Admin")]
        public MessageData DeletPostByAdmin(int postID)
        {
            try
            {
                var res = _postServices.DeletePostByAdmin(postID);
                return new MessageData { Data = res, Des = "Delete post success" };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
