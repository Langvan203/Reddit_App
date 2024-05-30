﻿using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
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

        public object UpdateInfo(UpdateUserInfoRequest request, int UserLoginID)
        {
            try
            {
                var res = _usersRepository.FindByCondition(p => p.UserID == UserLoginID).FirstOrDefault();
                if (res == null)
                {
                    return null;
                }
                else
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
                    res.PassWord = Utility.UtilityFunction.CreateMD5(request.PassWord);
                    res.Email = request.Email;
                    if(request.DateOfBirth == null)
                    {
                        res.DateOfBirth = DateTime.Now;
                    }    
                    else
                    {
                        res.DateOfBirth = request.DateOfBirth.Value;
                    }    
                    _usersRepository.UpdateByEntity(res);
                    _usersRepository.SaveChange();
                    return res;
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetUserLoginedInfo(int UserLoginID)
        {
            try
            {
                var res = _usersRepository.FindByCondition(u => u.UserID == UserLoginID).FirstOrDefault();
                return res;
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = "Can't not find user" };
            }
        }
        
        public object GetUerInfor(int UserID)
        {
            try
            {
                var res = _usersRepository.FindByCondition(u => u.UserID == UserID).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                return new MessageData { Data = null, Des = "Can't not find user" };
            }
        }
    }
}
