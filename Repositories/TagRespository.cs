using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;

namespace Reddit_App.Repositories
{
    public class TagRespository : BaseRespository<Tags>
    {
        private readonly IMapper _mapper;
        public TagRespository(ApiOptions options, DatabaseContext dbContext, IMapper mapper) :base(options, dbContext)
        {
            this._mapper = mapper;
        }
    }
}
