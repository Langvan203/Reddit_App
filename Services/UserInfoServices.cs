using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Repositories;
using Reddit_App.Request;
using System.IO;

namespace Reddit_App.Services
{
    public class UserInfoServices
    {
        private IMapper _mapper;

        private usersRepository _usersRepository;
        private ApiOptions _apiConfig;
        private IWebHostEnvironment _webHost;
        public UserInfoServices(ApiOptions apiConfig, DatabaseContext dbContext, IWebHostEnvironment webhost, IMapper mapper) 
        {
            _usersRepository = new usersRepository(apiConfig, dbContext, mapper);
            _mapper = mapper;
            _apiConfig = apiConfig;
            _webHost = webhost;
        }

        public MessageData UpdateInfo(UpdateUserInfoRequest request, int UserLoginID)
        {
            try
            {
                var res = _usersRepository.FindByCondition(p => p.UserID == UserLoginID).FirstOrDefault();
                if (res == null)
                {
                    return new MessageData { Data = null, Des = "Can't not get user info" };
                }
                else
                {
                    var checkUserName = _usersRepository.FindByCondition(u => u.UserName == request.UserName).FirstOrDefault();
                    var checkEmail = _usersRepository.FindByCondition(u => u.Email == request.Email).FirstOrDefault();

                    if(checkUserName != null && checkEmail != null)
                    {
                        if(request.Avata != null && request.Avata.FileName != res.Image)
                        {
                            var date = DateTime.UtcNow.ToString("yyyy_MM_dd");
                            using (FileStream fileStream = File.Create(_webHost.WebRootPath + "\\avata\\images" + date + request.Avata.FileName))
                            {
                                request.Avata.CopyTo(fileStream);
                                fileStream.Flush();
                            }
                            res.Image = "/avata/images" + date + request.Avata.FileName;
                        }    
                        res.PassWord = request.PassWord;
                        res.Email = request.Email;
                        res.DateOfBirth = request.DateOfBirth;
                        _usersRepository.UpdateByEntity(res);
                        _usersRepository.SaveChange();
                        return new MessageData { Data = res, Des = "Update user succes" };
                    }    
                    else
                    {
                        return new MessageData { Data = null, Des = "Email or username has been used" };
                    }    
                    
                }
            }
            catch (Exception ex)
            {
                return new MessageData { Data = null, Des = ex.Message };
            }
        }

        public MessageData GetInfo(int UserLoginID)
        {
            try
            {
                var res = _usersRepository.FindByCondition(u => u.UserID == UserLoginID).FirstOrDefault();
                return new MessageData { Data = res, Des = "Get user profile success" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "Can't not find user" };
            }
        }
    }
}
