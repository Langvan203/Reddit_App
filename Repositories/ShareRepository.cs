using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;

namespace Reddit_App.Repositories
{
    public class ShareRepository  : BaseRespository<Share>
    {
        private IMapper _mapper;
        
        public ShareRepository(ApiOptions apiOptions, DatabaseContext dbcontext, IMapper mapper) : base(apiOptions, dbcontext)
        {
            _mapper = mapper;
        }
          
     }
}
