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
    public class LikeController : BaseApiController<LikeController>
    {
        private readonly LikeServices _likeServices;
        private readonly IMapper _mapper;
        public LikeController(DatabaseContext context, ApiOptions apiOptions, IMapper mapper, IWebHostEnvironment webHost)
        {
            _mapper = mapper;
            _likeServices = new LikeServices(apiOptions, mapper, webHost, context);
        }

        [HttpGet]
        [Route("GetListLike")]
        public MessageData GetListLike(int postID)
        {
            try
            {
                var res = _likeServices.getListLike(postID);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }
        [HttpGet]
        [Route("GetNumberLike")]
        public MessageData GetNumberLike(int postID)
        {
            try
            {
                var res = _likeServices.getNumberLikePost(postID);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch (Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpPost]
        [Route("AddNewLike")]
        public MessageData AddNewLike(LikeRequest request)
        {
            try
            {
                var res = _likeServices.AddNewLike(request, UserIDLogined);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch (Exception e)
            {
                return NG(e);
            }
        }
        //[HttpPost]
        //[Route("DisLikePost")]
        //public MessageData DisLikePost(LikeRequest request)
        //{
        //    try
        //    {
        //        var res = _likeServices.DisLike(request, UserIDLogined);
        //        return new MessageData { Data = res.Data, Des = res.Des };
        //    }
        //    catch (Exception ex)
        //    {
        //        return NG(ex);
        //    }
        //} 
    }
}
