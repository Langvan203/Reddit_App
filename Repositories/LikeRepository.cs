using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;

namespace Reddit_App.Repositories
{
    public class LikeRepository : BaseRespository<Like>
    {
        private IMapper _mapper;
        public LikeRepository(ApiOptions apiOptions, DatabaseContext dbContext, IMapper mapper) : base(apiOptions, dbContext)
        {
            _mapper = mapper;
        }
    }
}
