using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;

namespace Reddit_App.Services
{
    public class TagServices
    {
        private readonly TagRespository _tagRepos;
        private readonly ApiOptions _apiOptions;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHost;
        public TagServices(ApiOptions apiOptions, DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webhost) 
        {
            _tagRepos = new TagRespository(apiOptions, dbContext, mapper);
            _mapper = mapper;
            _webHost = webhost;
            _apiOptions = apiOptions;
        }

        public MessageData AddNewTag(TagRequest request)
        {
            try
            {
                var res = _tagRepos.FindAll().Where(c => c.TagName == request.TagName).FirstOrDefault();
                if(res == null)
                {
                    var newTag = _mapper.Map<Tags>(request);
                    _tagRepos.Create(newTag);
                    _tagRepos.SaveChange();
                    return new MessageData { Data = newTag, Des = "Add new tag succesfully" };
                }    
                else
                {
                    return new MessageData { Data = null, Des = $"{request.TagName} has been already in Tags" };
                }    
            }
            catch
            {
                return new MessageData { Data = null, Des = "Error" };
            }
        }
    }
}
