using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;
using MySqlConnector.Authentication;

namespace Reddit_App.Services
{
    public class CommentServices
    {
        private readonly CommentRepository _commentRepository;
        private readonly usersRepository _userRepository;
        private readonly ApiOptions _apiOptions;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHost;

        public CommentServices(ApiOptions apiOptions, DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHost)
        {
            _commentRepository = new CommentRepository(apiOptions, dbContext, mapper);
            _userRepository = new usersRepository(apiOptions, dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper;
            _webHost = webHost;
        }

        public object AddNewComment(int UserLogined, NewCommentRequest request)
        {
            try
            {
                var newComment = _mapper.Map<Comment>(request);
                newComment.Image = "";
                if (request.ImageURL != null)
                {
                    var date = DateTime.UtcNow.ToString("yyyy_MM_dd");
                    using(FileStream fileStream = File.Create(_webHost.WebRootPath + "\\comment\\images\\" + date + request.ImageURL.FileName))
                    {
                        request.ImageURL.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                    newComment.Image = "comment/images/" + date + request.ImageURL.FileName;
                }
                newComment.CommentStatus = 1;
                newComment.UserID = UserLogined;
                _commentRepository.Create(newComment);
                _commentRepository.SaveChange();
                return newComment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetListComment(int PostID)
        {
            try
            {
                var listComment = _commentRepository.FindByCondition(m => m.PostID == PostID && m.CommentStatus == 1).ToList();
                var res = listComment.Select(p => p.UserID).ToList();
                var users = _userRepository.FindByCondition(p => res.Contains(p.UserID)).ToList();
                List<ListCommentDto> listUserComment= new List<ListCommentDto>();
                foreach(var item in listComment)
                {
                    var userComment = new ListCommentDto();
                    userComment.Content = item.Content;
                    userComment.CommentImage = item.Image;
                    var checkUserInfor = users.FirstOrDefault(p => p.UserID == item.UserID);
                    if (checkUserInfor != null)
                    {
                        userComment.UserID = checkUserInfor.UserID;
                        userComment.UserName = checkUserInfor.UserName;
                        userComment.Avata = checkUserInfor.Image;
                    }
                    listUserComment.Add(userComment);
                }
                listUserComment.Reverse();
                return new
                {
                    ListComment = listUserComment,
                    NumberComment = listUserComment.Count()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object UpdateComment(int UserLogined, NewCommentRequest request)
        {
            try
            {
                var uComment = _commentRepository.FindByCondition(m => m.UserID == UserLogined && m.PostID == request.PostID && m.CommentStatus == 1).FirstOrDefault();
                if (uComment != null)
                {
                    uComment.Content = request.Content;
                    _commentRepository.UpdateByEntity(uComment);
                    _commentRepository.SaveChange();
                    return uComment;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        public object DeleteComment(int UserLogined, NewCommentRequest request)
        {
            try
            {
                
                var dComment = _commentRepository.FindByCondition(m => m.UserID == UserLogined && m.PostID == request.PostID && m.CommentStatus == 1).FirstOrDefault();
                if(dComment != null)
                {
                    dComment.CommentStatus = 0;
                    _commentRepository.UpdateByEntity(dComment);
                    _commentRepository.SaveChange();
                    return dComment;
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
