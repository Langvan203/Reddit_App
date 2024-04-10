using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Models;
using Reddit_App.Request;
using Reddit_App.Utility;

namespace Reddit_App.Repositories
{
    public class usersRepository : BaseRespository<users>
    {
        private IMapper _mapper;
        public usersRepository(ApiOptions apiConfig, DatabaseContext dbContext, IMapper mapper) : base(apiConfig, dbContext) 
        {
            this._mapper = mapper;   
        }
        public users UserLogin(LoginRequest userloginRequest)
        {
            try
            {
                //var passwordbyMD5 = "admin";
                return Model.Where(c => c.UserName == userloginRequest.UserName && c.PassWord == userloginRequest.PassWord).FirstOrDefault();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
