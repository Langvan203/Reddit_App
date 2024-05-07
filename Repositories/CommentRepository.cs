using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;

namespace Reddit_App.Repositories
{
    public class CommentRepository : BaseRespository<Comment>
    {
        private readonly IMapper _mapper;

        public CommentRepository(ApiOptions apiOption, DatabaseContext dbContext, IMapper mapper) : base(apiOption,dbContext)
        {
            _mapper = mapper;
        }
    }
}
