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

        public object AddNewTag(TagRequest request)
        {
            try
            {
                var res = _tagRepos.FindByCondition(t => t.TagName == request.TagName).FirstOrDefault();
                if(res == null)
                {
                    var newTag = _mapper.Map<Tags>(request);
                    newTag.TagStatus = 1;
                    _tagRepos.Create(newTag);
                    _tagRepos.SaveChange();
                    return newTag;
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


        public object GetListTag()
        {
            try
            {
                var res = _tagRepos.FindByCondition(t => t.TagStatus == 1);
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public object RemoveTag(int TagID)
        {
            try
            {
                var res = _tagRepos.FindByCondition(r => r.TagID == TagID).FirstOrDefault();
                if (res != null)
                {
                    _tagRepos.DeleteByEntity(res);
                    _tagRepos.SaveChange();
                    return res;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public object GetListTagByAdmin()
        {
            try
            {
                var res = _tagRepos.FindAll();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
