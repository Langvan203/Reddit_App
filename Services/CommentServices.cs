using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;

namespace Reddit_App.Services
{
    public class CommentServices
    {
        private readonly CommentRepository _commentRepository;
        private readonly ApiOptions _apiOptions;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHost;

        public CommentServices(ApiOptions apiOptions, DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webHost)
        {
            _commentRepository = new CommentRepository(apiOptions, dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper;
            _webHost = webHost;
        }

        public MessageData AddNewComment(int UserLogined, NewCommentRequest request)
        {
            try
            {
                var newComment = _mapper.Map<Comment>(request);
                newComment.CommentStatus = 1;
                newComment.UserID = UserLogined;
                _commentRepository.Create(newComment);
                _commentRepository.SaveChange();
                return new MessageData { Data = newComment, Des = "Add new comment success" };
            }
            catch (Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        public MessageData GetListComment(int PostID)
        {
            try
            {
                var listComment = _commentRepository.FindByCondition(m => m.PostID == PostID && m.CommentStatus == 1).Select(l => new
                {
                    UserID = l.UserID,
                    Content = l.Content
                });
                return new MessageData { Data = listComment, Des = "Get list comment success" };
            }
            catch (Exception ex)
            {
                return new MessageData { Data = null, Des = "Error when get list comment" };
            }
        }

        public MessageData GetTotalComment(int PostID)
        {
            try
            {
                var totalComment = _commentRepository.FindByCondition(m => m.PostID == PostID && m.CommentStatus == 1).GroupBy(m => m.PostID).Select(l => new
                {
                    PostID = l.Key,
                    TotalComment = l.Count()
                });
                return new MessageData { Data = totalComment, Des = "Get total comment success" };
            }
            catch (Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        public MessageData UpdateComment(int UserLogined, NewCommentRequest request)
        {
            try
            {
                var uComment = _commentRepository.FindByCondition(m => m.UserID == UserLogined && m.PostID == request.PostID && m.CommentStatus == 1).FirstOrDefault();
                if (uComment != null)
                {
                    uComment.Content = request.Content;
                    _commentRepository.UpdateByEntity(uComment);
                    _commentRepository.SaveChange();
                    return new MessageData { Data = uComment, Des = "Update comment success" };
                }
                else
                {
                    return new MessageData { Data = null, Des = "Can't find comment" };
                }
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "Error when update comment" };
            }
        }
        
        public MessageData DeleteComment(int UserLogined, NewCommentRequest request)
        {
            try
            {
                
                var dComment = _commentRepository.FindByCondition(m => m.UserID == UserLogined && m.PostID == request.PostID && m.CommentStatus == 1).FirstOrDefault();
                if(dComment != null)
                {
                    dComment.CommentStatus = 0;
                    _commentRepository.UpdateByEntity(dComment);
                    _commentRepository.SaveChange();
                    return new MessageData { Data = dComment, Des = "Delete comment succes" };
                }
                return new MessageData { Data = null, Des = "Can't find comment" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message};
            }
        }

        
    }
}
