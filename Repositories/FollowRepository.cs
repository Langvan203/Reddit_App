using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;

namespace Reddit_App.Repositories
{
    public class FollowRepository : BaseRespository<Follow>
    {
        private readonly IMapper _mapper;
        public FollowRepository(ApiOptions apiConfig, DatabaseContext dbContext, IMapper mapper) : base(apiConfig, dbContext)
        {
            _mapper = mapper;
        }   
    }
}
