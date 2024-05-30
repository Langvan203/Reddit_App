using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Repositories;
using Reddit_App.Request;
using System.Net.WebSockets;
using Reddit_App.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;


namespace Reddit_App.Services
{
    public class PostServices
    {
        private readonly PostRespository _postRespository;
        private readonly usersRepository _userRepository;
        private readonly TagRespository _tagRepository;
        private readonly CommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webhost;
        private readonly ApiOptions _apiOptions;

        public PostServices(ApiOptions apiOptions,DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webhost)
        {
            _postRespository = new PostRespository(apiOptions,dbContext, mapper);
            _userRepository = new usersRepository(apiOptions, dbContext, mapper);
            _tagRepository = new TagRespository(apiOptions, dbContext, mapper);
            _commentRepository = new CommentRepository(apiOptions, dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper; 
            _webhost = webhost;
        }

        public async Task<object> AddNewPost(CreateNewPost request, int userID)
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

                return p;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public object GetAllPost()
        {
            try
            {
                var res = _postRespository.FindAll().ToList();
                var listUser = res.Select(p => p.UserID).ToList();
                var listTag = res.Select(p => p.TagID).ToList();
                var userPost = _userRepository.FindByCondition(p => listUser.Contains(p.UserID)).ToList();
                var tagPost = _tagRepository.FindByCondition(t => listTag.Contains(t.TagID)).ToList();
                var listComment = res.Select(p => p.PostID).ToList();
                var comments = _commentRepository.FindByCondition(m => listComment.Contains(m.PostID)).ToList();
                var commentsCount = comments.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());
                List<GetPostDto> listPost = new List<GetPostDto>();
                foreach(var item in res)
                {
                    var totalComment = commentsCount.ContainsKey(item.PostID) ? commentsCount[item.PostID] : 0;
                    var dsPost = new GetPostDto();
                    dsPost.PostID = item.PostID;
                    dsPost.Content = item.Content;
                    dsPost.Title = item.Title;
                    dsPost.Image = item.Image;
                    var checkUser = userPost.FirstOrDefault(p => p.UserID == item.UserID);
                    if(checkUser != null)
                    {
                        dsPost.UserID = checkUser.UserID;
                        dsPost.UserName = checkUser.UserName;
                        dsPost.Avata = checkUser.Image;
                    }
                    var checkTag = tagPost.FirstOrDefault(t => t.TagID == item.TagID);
                    if(checkTag != null)
                    {
                        dsPost.TagName = checkTag.TagName;
                    }
                    dsPost.TotalComment = totalComment;
                    listPost.Add(dsPost);
                }
                listPost.Reverse();
                return listPost;
            }
            catch
            {
                return new MessageData { Data = null, Des = "get all post failed" };
            }
        }
        
        public object GetPostByUser(int userID)
        {
            try
            {
                var res = _postRespository.FindAll().Where(c => c.UserID == userID);
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object UpdatePost(UpdatePostRequest request,int userID)
        {
            try
            {
                // kiểm tra xem nếu user đó là người đăng bài thì mới cho phép cập nhật lại bài viết của mình
                var postUpdate = _postRespository.FindByCondition(p => p.PostID == request.PostID && p.UserID == userID).FirstOrDefault();
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
                return postUpdate;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object GetPostByTag(int tagID)
        {
            try
            {
                var res = _postRespository.FindAll().Where(p => p.TagID == tagID);
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // khi tìm kiếm bằng UTF-8 và khi không có thì lỗi tìm 
        public object GetPostByContent(string content, string title)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.Content.Contains(content) || p.Title.Contains(title));
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object DeletePostByID(int PostID)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.PostID == PostID).FirstOrDefault();
                if(res == null)
                {
                    return null;
                }    
                else
                {
                    _postRespository.DeleteByEntity(res);
                    _postRespository.SaveChange();
                    return res;
                }    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
