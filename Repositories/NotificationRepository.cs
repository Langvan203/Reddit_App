using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;

namespace Reddit_App.Repositories
{
    public class NotificationRepository : BaseRespository<Notifications>
    {
        private readonly IMapper _mapper;
        public NotificationRepository(ApiOptions apiConfig, DatabaseContext dbContext, IMapper mapper) : base(apiConfig, dbContext)
        {
            this._mapper = mapper;
        }
    }
}
