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
    public class TagController : BaseApiController<TagController>
    {
        private readonly TagServices _tagServices;
        private IMapper _mapper;
        public TagController(ApiOptions apiOption, DatabaseContext dbContetxt, IMapper mapper, IWebHostEnvironment webhost)
        {
            _mapper = mapper;
            _tagServices = new TagServices(apiOption, dbContetxt, mapper, webhost);
        }

        [HttpPost]
        [Route("AddNewTag")]
        public MessageData AddNewTag(TagRequest request)
        {
            try
            {
                var res = _tagServices.AddNewTag(request);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpGet]
        [Route("GetListTag")]
        public MessageData GetListTag()
        {
            try
            {
                var res = _tagServices.GetListTag();
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }

        [HttpDelete]
        [Route("DeleteTag")]
        public MessageData RemoveTag(int TagID)
        {
            try
            {
                var res = _tagServices.RemoveTag(TagID);
                return new MessageData { Data = res.Data, Des = res.Des };
            }
            catch(Exception ex)
            {
                return NG(ex);
            }
        }
    }
}
