using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;
namespace Reddit_App.Repositories
{
    public class PostRespository : BaseRespository<Post>
    {
        private IMapper _mapper;
        public PostRespository(ApiOptions apiConfig,DatabaseContext databaseContext, IMapper mapper) : base(apiConfig, databaseContext)
        {
            _mapper = mapper;
        }
    }
}
