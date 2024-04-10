using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Repositories;
using Reddit_App.Request;
using System.Net.WebSockets;
using Reddit_App.Models;

namespace Reddit_App.Services
{
    public class PostServices
    {
        private readonly PostRespository _postRespository;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webhost;
        private readonly ApiOptions _apiOptions;

        public PostServices(ApiOptions apiOptions,DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webhost)
        {
            _postRespository = new PostRespository(apiOptions,dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper; 
            _webhost = webhost;
        }

        public MessageData AddNewPost(CreateNewPost request, int userID)
        {
            try
            {
                var p = _mapper.Map<Post>(request);
                p.Image = "";
                if(request.Image != null)
                {
                    var date = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm");
                    using(FileStream fileStream = File.Create(_webhost.WebRootPath + "\\posts\\images\\" + date + request.Image.FileName))
                    {
                        request.Image.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                    p.Image = "posts/images/" + date + request.Image.FileName;
                }
                p.UserID = userID;
                _postRespository.Create(p);
                _postRespository.SaveChange();
                return new MessageData { Data = p, Des = "Add new post succesfully" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "Fail to add new post" };
            }
        }
        public MessageData GetAllPost()
        {
            try
            {
                var res = _postRespository.FindAll();
                return new MessageData { Data = res, Des = "Get all post" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "get all post failed" };
            }
        }
        
        public MessageData GetPostByUser(int userID)
        {
            try
            {
                var res = _postRespository.FindAll().Where(c => c.UserID == userID);
                return new MessageData { Data = res, Des = "Get success" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "Get failed" };
            }
        }

        public MessageData UpdatePost(int PostID, CreateNewPost request, int userID)
        {
            try
            {
                var postUpdate = _postRespository.FindByCondition(p => p.PostID == PostID).FirstOrDefault();
                if(postUpdate == null)
                {
                    return new MessageData { Data = null, Des = "Can't not find post" };
                }    
                if(request.Image != null && request.Image.FileName != postUpdate.Image)
                {
                    var date = DateTime.UtcNow.ToString("yyyy_MM_dd");
                    using(FileStream fileStream = File.Create(_webhost.WebRootPath + "\\posts\\images\\" + date + request.Image.FileName))
                    {
                        request.Image.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                    postUpdate.Image = "posts/images/" + date + request.Image.FileName;      
                }    
                postUpdate.TagID = request.TagID;
                postUpdate.Title = request.Title;
                postUpdate.Content = request.Content;
                postUpdate.UserID = userID;
                _postRespository.UpdateByEntity(postUpdate);
                _postRespository.SaveChange();
                return new MessageData { Data = postUpdate, Des = "Update successfully" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "update failed" };
            }
        }

        public MessageData GetPostByTag(int tagID)
        {
            try
            {
                var res = _postRespository.FindAll().Where(p => p.TagID == tagID);
                return new MessageData { Data = res, Des = $"Find all post with tagID = {tagID} successfully" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "Find faild" };
            }
        }
    }
}
