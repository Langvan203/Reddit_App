using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Repositories;
using Reddit_App.Request;
using System.Net.WebSockets;
using Reddit_App.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.AspNetCore.SignalR;
using Reddit_App.Helpers.SendnotificationHub;
using System.Text.Json;
using NetTopologySuite.Index.HPRtree;




namespace Reddit_App.Services
{
    public class PostServices
    {
        private readonly PostRespository _postRespository;
        private readonly usersRepository _userRepository;
        private readonly TagRespository _tagRepository;
        private readonly CommentRepository _commentRepository;
        private readonly LikeRepository _likeRepository;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webhost;
        private readonly ApiOptions _apiOptions;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly FollowRepository _followRepository;

        public PostServices(ApiOptions apiOptions,DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webhost, IHubContext<NotificationHub> hubContext)
        {
            _postRespository = new PostRespository(apiOptions,dbContext, mapper);
            _userRepository = new usersRepository(apiOptions, dbContext, mapper);
            _tagRepository = new TagRespository(apiOptions, dbContext, mapper);
            _commentRepository = new CommentRepository(apiOptions, dbContext, mapper);
            _likeRepository = new LikeRepository(apiOptions, dbContext, mapper);
            _followRepository = new FollowRepository(apiOptions, dbContext, mapper);
            _apiOptions = apiOptions;
            _mapper = mapper; 
            _webhost = webhost;
            _hubContext = hubContext;
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
                p.PostStatus = 1;
                p.TagID = JsonSerializer.Serialize(request.TagID);
                _postRespository.Create(p);
                _postRespository.SaveChange();
                var followers = _followRepository.FindByCondition(f => f.FollowedID == userID && f.Status == 1).ToList();
                foreach(var item in followers)
                {
                    var message = $"User {userID} has create a new post";
                    await _hubContext.Clients.User(item.FollowedID.ToString()).SendAsync("ReceiveNotification", message);
                }    
                return p;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object GetListPost()
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.PostStatus == 1).ToList();
                var listUser = res.Select(p => p.UserID).ToList();
                var listTag = res.Select(p => p.TagID).ToList();
                var userPost = _userRepository.FindByCondition(p => listUser.Contains(p.UserID)).ToList();
                var allTags = _tagRepository.FindAll().ToList();

                // get list comment by postid
                var listComment = res.Select(p => p.PostID).ToList();
                var comments = _commentRepository.FindByCondition(m => listComment.Contains(m.PostID)).ToList();
                var userInComment = comments.Select(m => m.UserID).ToList();
                var allUserIncomment = _userRepository.FindByCondition(p => userInComment.Contains(p.UserID)).ToList();
                var commentsCount = comments.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());

                // get list like by postid
                var listLike = res.Select(p => p.PostID).ToList();
                var likes = _likeRepository.FindByCondition(m => listLike.Contains(m.PostID)).ToList();
                var likeCount = likes.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());
                var LuserLike = likes.Select(p => p.UserID).ToList();
                var allUserInLike = _userRepository.FindByCondition(p => LuserLike.Contains(p.UserID)).ToList();
                List<GetPostDto> listPost = new List<GetPostDto>();
                
                foreach (var item in res)
                {
                    var totalComment = commentsCount.ContainsKey(item.PostID) ? commentsCount[item.PostID] : 0;
                    var totalLike = likeCount.ContainsKey(item.PostID) ? likeCount[item.PostID] : 0;
                    var dsPost = new GetPostDto();
                    dsPost.PostID = item.PostID;
                    dsPost.Content = item.Content;
                    dsPost.Title = item.Title;
                    dsPost.Image = item.Image;
                    dsPost.CreatedDate = item.CreatedDate;
                    dsPost.UpdatedDate = item.UpdatedDate;
                    var checkUser = userPost.FirstOrDefault(p => p.UserID == item.UserID);
                    if (checkUser != null)
                    {
                        dsPost.UserID = checkUser.UserID;
                        dsPost.UserName = checkUser.UserName;
                        dsPost.Avata = checkUser.Image;
                    }

                    // get all list comment in post
                    var listcomment = comments.Where(t => t.PostID == item.PostID).Select(t => new GetListCommentPostDto
                    {
                        Content = t.Content,
                        UserID = t.UserID,
                        UserName = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.UserName).FirstOrDefault(),
                        Avata = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.Image).FirstOrDefault(),
                        CommentID = t.CommentID
                    }).ToList();
                    dsPost.ListComment = listcomment;

                    // get all tag list in post
                    var tagIDs = JsonSerializer.Deserialize<List<int>>(item.TagID);
                    var tagNames = allTags.Where(t => tagIDs.Contains(t.TagID)).Select(t => new TagDto
                    {
                        ID = t.TagID,
                        Name = t.TagName
                    }).ToList();


                    // get all like in post
                    var listlike = likes.Where(l => l.PostID == item.PostID).Select(l => new GetLikeInPostDto
                    {
                        UserID = allUserInLike.Where(u => u.UserID == l.UserID).Select(u => u.UserID).FirstOrDefault()
                    }).ToList();
                    dsPost.ListLike = listlike;

                    dsPost.ListTag = tagNames;
                    dsPost.TotalComment = totalComment;
                    dsPost.TotalLike = totalLike;
                    listPost.Add(dsPost);
                }
                listPost.Reverse();
                return listPost;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public object GetPostByID(int PostID)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.PostID == PostID && p.PostStatus == 1);
                var listUser = res.Select(p => p.UserID).ToList();
                var listTag = res.Select(p => p.TagID).ToList();
                var userPost = _userRepository.FindByCondition(p => listUser.Contains(p.UserID)).ToList();
                var allTags = _tagRepository.FindAll().ToList();

                // get list comment by postid
                var listComment = res.Select(p => p.PostID).ToList();
                var comments = _commentRepository.FindByCondition(m => listComment.Contains(m.PostID)).ToList();
                var userInComment = comments.Select(m => m.UserID).ToList();
                var allUserIncomment = _userRepository.FindByCondition(p => userInComment.Contains(p.UserID)).ToList();
                var commentsCount = comments.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());

                // get list like by postid
                var listLike = res.Select(p => p.PostID).ToList();
                var likes = _likeRepository.FindByCondition(m => listLike.Contains(m.PostID)).ToList();
                var likeCount = likes.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());
                var LuserLike = likes.Select(p => p.UserID).ToList();
                var allUserInLike = _userRepository.FindByCondition(p => LuserLike.Contains(p.UserID)).ToList();
                List<GetPostDto> listPost = new List<GetPostDto>();

                foreach (var item in res)
                {
                    var totalComment = commentsCount.ContainsKey(item.PostID) ? commentsCount[item.PostID] : 0;
                    var totalLike = likeCount.ContainsKey(item.PostID) ? likeCount[item.PostID] : 0;
                    var dsPost = new GetPostDto();
                    dsPost.PostID = item.PostID;
                    dsPost.Content = item.Content;
                    dsPost.Title = item.Title;
                    dsPost.Image = item.Image;
                    dsPost.CreatedDate = item.CreatedDate;
                    dsPost.UpdatedDate = item.UpdatedDate;
                    var checkUser = userPost.FirstOrDefault(p => p.UserID == item.UserID);
                    if (checkUser != null)
                    {
                        dsPost.UserID = checkUser.UserID;
                        dsPost.UserName = checkUser.UserName;
                        dsPost.Avata = checkUser.Image;
                    }

                    // get all list comment in post
                    var listcomment = comments.Where(t => t.PostID == item.PostID).Select(t => new GetListCommentPostDto
                    {
                        Content = t.Content,
                        UserID = t.UserID,
                        UserName = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.UserName).FirstOrDefault(),
                        Avata = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.Image).FirstOrDefault(),
                        CommentID = t.CommentID
                    }).ToList();
                    dsPost.ListComment = listcomment;

                    // get all tag list in post
                    var tagIDs = System.Text.Json.JsonSerializer.Deserialize<List<int>>(item.TagID);
                    var tagNames = allTags.Where(t => tagIDs.Contains(t.TagID)).Select(t => new TagDto
                    {
                        ID = t.TagID,
                        Name = t.TagName
                    }).ToList();


                    // get all like in post
                    var listlike = likes.Where(l => l.PostID == item.PostID).Select(l => new GetLikeInPostDto
                    {
                        UserID = allUserInLike.Where(u => u.UserID == l.UserID).Select(u => u.UserID).FirstOrDefault()
                    }).ToList();
                    dsPost.ListLike = listlike;

                    dsPost.ListTag = tagNames;
                    dsPost.TotalComment = totalComment;
                    dsPost.TotalLike = totalLike;
                    listPost.Add(dsPost);
                }
                listPost.Reverse();
                return listPost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetPostByUser(int userID)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.PostStatus == 1 && p.UserID == userID);
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object UpdatePost(UpdatePostRequest request, int userID)
        {
            try
            {
                // kiểm tra xem nếu user đó là người đăng bài thì mới cho phép cập nhật lại bài viết của mình
                var postUpdate = _postRespository.FindByCondition(p => p.PostID == request.PostID && p.UserID == userID && p.PostStatus == 1).FirstOrDefault();
                if (postUpdate == null)
                {
                    return new MessageData { Data = null, Des = "Can't not find post" };
                }
                if (request.Image != null && request.Image.FileName != postUpdate.Image)
                {
                    var date = DateTime.UtcNow.ToString("yyyy_MM_dd");
                    using (FileStream fileStream = File.Create(_webhost.WebRootPath + "\\posts\\images\\" + date + request.Image.FileName))
                    {
                        request.Image.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                    postUpdate.Image = "posts/images/" + date + request.Image.FileName;
                }
                postUpdate.TagID = JsonSerializer.Serialize(request.TagID);
                postUpdate.Title = request.Title;
                postUpdate.Content = request.Content;
                postUpdate.UserID = userID;
                postUpdate.UpdatedDate = DateTime.UtcNow;
                _postRespository.UpdateByEntity(postUpdate);
                _postRespository.SaveChange();
                return postUpdate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // khi tìm kiếm bằng UTF-8 và khi không có thì lỗi tìm 
        public object GetPostByContent(string content)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.Content.Contains(content) || p.Title.Contains(content));
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object DeletePostByID(int PostID, int UserCreatedPost)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.PostID == PostID && p.UserID == UserCreatedPost && p.PostStatus == 1).FirstOrDefault();
                if(res == null)
                {
                    return null;
                }    
                else
                {
                    res.PostStatus = 0;
                    _postRespository.UpdateByEntity(res);
                    _postRespository.SaveChange();
                    return res;
                }    
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object GetPostByTag(int TagID)
        {
            try
            {
                var posts = _postRespository.FindByCondition(p => p.PostStatus == 1).ToList();
                var res = posts.Where(t => (Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(t.TagID).Contains(TagID))).ToList();
                var listUser = res.Select(p => p.UserID).ToList();
                var listTag = res.Select(p => p.TagID).ToList();
                var userPost = _userRepository.FindByCondition(p => listUser.Contains(p.UserID)).ToList();
                var allTags = _tagRepository.FindAll().ToList();

                // get list comment by postid
                var listComment = res.Select(p => p.PostID).ToList();
                var comments = _commentRepository.FindByCondition(m => listComment.Contains(m.PostID)).ToList();
                var userInComment = comments.Select(m => m.UserID).ToList();
                var allUserIncomment = _userRepository.FindByCondition(p => userInComment.Contains(p.UserID)).ToList();
                var commentsCount = comments.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());

                // get list like by postid
                var listLike = res.Select(p => p.PostID).ToList();
                var likes = _likeRepository.FindByCondition(m => listLike.Contains(m.PostID)).ToList();
                var likeCount = likes.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());
                var LuserLike = likes.Select(p => p.UserID).ToList();
                var allUserInLike = _userRepository.FindByCondition(p => LuserLike.Contains(p.UserID)).ToList();
                List<GetPostDto> listPost = new List<GetPostDto>();

                foreach (var item in res)
                {
                    var totalComment = commentsCount.ContainsKey(item.PostID) ? commentsCount[item.PostID] : 0;
                    var totalLike = likeCount.ContainsKey(item.PostID) ? likeCount[item.PostID] : 0;
                    var dsPost = new GetPostDto();
                    dsPost.PostID = item.PostID;
                    dsPost.Content = item.Content;
                    dsPost.Title = item.Title;
                    dsPost.Image = item.Image;
                    dsPost.CreatedDate = item.CreatedDate;
                    dsPost.UpdatedDate = item.UpdatedDate;
                    var checkUser = userPost.FirstOrDefault(p => p.UserID == item.UserID);
                    if (checkUser != null)
                    {
                        dsPost.UserID = checkUser.UserID;
                        dsPost.UserName = checkUser.UserName;
                        dsPost.Avata = checkUser.Image;
                    }

                    // get all list comment in post
                    var listcomment = comments.Where(t => t.PostID == item.PostID).Select(t => new GetListCommentPostDto
                    {
                        Content = t.Content,
                        UserID = t.UserID,
                        UserName = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.UserName).FirstOrDefault(),
                        Avata = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.Image).FirstOrDefault(),
                        CommentID = t.CommentID
                    }).ToList();
                    dsPost.ListComment = listcomment;

                    // get all tag list in post
                    var tagIDs = System.Text.Json.JsonSerializer.Deserialize<List<int>>(item.TagID);
                    var tagNames = allTags.Where(t => tagIDs.Contains(t.TagID)).Select(t => new TagDto
                    {
                        ID = t.TagID,
                        Name = t.TagName
                    }).ToList();


                    // get all like in post
                    var listlike = likes.Where(l => l.PostID == item.PostID).Select(l => new GetLikeInPostDto
                    {
                        UserID = allUserInLike.Where(u => u.UserID == l.UserID).Select(u => u.UserID).FirstOrDefault()
                    }).ToList();
                    dsPost.ListLike = listlike;

                    dsPost.ListTag = tagNames;
                    dsPost.TotalComment = totalComment;
                    dsPost.TotalLike = totalLike;
                    listPost.Add(dsPost);
                }
                listPost.Reverse();
                return listPost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object DeletePostByAdmin(int PostID)
        {
            try
            {
                var res = _postRespository.FindByCondition(p => p.PostID == PostID).FirstOrDefault();
                if (res == null)
                {
                    return null;
                }
                else
                {
                    res.PostStatus = 0;
                    _postRespository.UpdateByEntity(res);
                    _postRespository.SaveChange();
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public object GetListPostByAdmin()
        {
            try
            {
                var res = _postRespository.FindAll().ToList();
                var listUser = res.Select(p => p.UserID).ToList();
                var listTag = res.Select(p => p.TagID).ToList();
                var userPost = _userRepository.FindByCondition(p => listUser.Contains(p.UserID)).ToList();
                var allTags = _tagRepository.FindAll().ToList();

                // get list comment by postid
                var listComment = res.Select(p => p.PostID).ToList();
                var comments = _commentRepository.FindByCondition(m => listComment.Contains(m.PostID)).ToList();
                var userInComment = comments.Select(m => m.UserID).ToList();
                var allUserIncomment = _userRepository.FindByCondition(p => userInComment.Contains(p.UserID)).ToList();
                var commentsCount = comments.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());

                // get list like by postid
                var listLike = res.Select(p => p.PostID).ToList();
                var likes = _likeRepository.FindByCondition(m => listLike.Contains(m.PostID)).ToList();
                var likeCount = likes.GroupBy(c => c.PostID).ToDictionary(g => g.Key, g => g.Count());
                var LuserLike = likes.Select(p => p.UserID).ToList();
                var allUserInLike = _userRepository.FindByCondition(p => LuserLike.Contains(p.UserID)).ToList();
                List<GetPostDto> listPost = new List<GetPostDto>();

                foreach (var item in res)
                {
                    var totalComment = commentsCount.ContainsKey(item.PostID) ? commentsCount[item.PostID] : 0;
                    var totalLike = likeCount.ContainsKey(item.PostID) ? likeCount[item.PostID] : 0;
                    var dsPost = new GetPostDto();
                    dsPost.PostID = item.PostID;
                    dsPost.Content = item.Content;
                    dsPost.Title = item.Title;
                    dsPost.Image = item.Image;
                    dsPost.CreatedDate = item.CreatedDate;
                    dsPost.UpdatedDate = item.UpdatedDate;
                    var checkUser = userPost.FirstOrDefault(p => p.UserID == item.UserID);
                    if (checkUser != null)
                    {
                        dsPost.UserID = checkUser.UserID;
                        dsPost.UserName = checkUser.UserName;
                        dsPost.Avata = checkUser.Image;
                    }

                    // get all list comment in post
                    var listcomment = comments.Where(t => t.PostID == item.PostID).Select(t => new GetListCommentPostDto
                    {
                        Content = t.Content,
                        UserID = t.UserID,
                        UserName = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.UserName).FirstOrDefault(),
                        Avata = allUserIncomment.Where(u => u.UserID == t.UserID).Select(u => u.Image).FirstOrDefault(),
                        CommentID = t.CommentID
                    }).ToList();
                    dsPost.ListComment = listcomment;

                    // get all tag list in post
                    var tagIDs = JsonSerializer.Deserialize<List<int>>(item.TagID);
                    var tagNames = allTags.Where(t => tagIDs.Contains(t.TagID)).Select(t => new TagDto
                    {
                        ID = t.TagID,
                        Name = t.TagName
                    }).ToList();


                    // get all like in post
                    var listlike = likes.Where(l => l.PostID == item.PostID).Select(l => new GetLikeInPostDto
                    {
                        UserID = allUserInLike.Where(u => u.UserID == l.UserID).Select(u => u.UserID).FirstOrDefault()
                    }).ToList();
                    dsPost.ListLike = listlike;

                    dsPost.ListTag = tagNames;
                    dsPost.TotalComment = totalComment;
                    dsPost.TotalLike = totalLike;
                    listPost.Add(dsPost);
                }
                listPost.Reverse();
                return listPost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
