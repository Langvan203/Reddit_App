using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Models;
using Reddit_App.Repositories;
using Reddit_App.Request;
using Reddit_App.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Reddit_App.Services
{
    public class UserAuthenticateService
    {
        private readonly usersRepository _userRepository;
        private readonly ApiOptions _apiOption;
        private readonly IMapper _mapper;

        public UserAuthenticateService(ApiOptions apiOption,DatabaseContext dbContext, IMapper mapper)
        {
            _userRepository = new usersRepository(apiOption, dbContext, mapper);
            _apiOption = apiOption;
            _mapper = mapper;
        }

        public object UserLogin(LoginRequest request)
        {
            try
            {
                var user = _userRepository.UserLogin(request);
                if(user == null)
                {
                    throw new ValidateError(1004, "Unactivated account");
                }  
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiOption.Secret));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                
                var claimList = new[]
                {
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.UserData, user.UserName),
                    new Claim(ClaimTypes.Sid, user.UserID.ToString()),
                };
                //if (user.UserName == "admin" && user.PassWord == "admin")
                //{
                //    var newClaimList = claimList.ToList();
                //    //newClaimList.RemoveAll(claim => claim.Type == ClaimTypes.Role && claim.Value == "User");
                //    newClaimList.Add(new Claim(ClaimTypes.Role, "Admin"));
                //    // renew claim
                //    claimList = newClaimList.ToArray();
                //}
                var token = new JwtSecurityToken(
                    issuer: _apiOption.ValidIssuer,
                    audience: _apiOption.ValidAudience,
                    expires: DateTime.Now.AddDays(1),
                    claims: claimList,
                    signingCredentials: credentials
                    );
                var tokenByString = new JwtSecurityTokenHandler().WriteToken(token);
                return new
                {
                    token = tokenByString,
                    users = user
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MessageData UserRegister(UserRegisterRequest request)
        {
            try
            {
                var findUser = _userRepository.FindByCondition(r => r.UserName == request.UserName).FirstOrDefault();
                if(findUser != null)
                {
                    return new MessageData { Data = null, Des = "User name has been used" };
                }



                //var newUser = new users()
                //{
                //    UserName = request.UserName,
                //    PassWord = Utility.UtilityFunction.CreateMD5(request.PassWord),
                //    Email = request.Email,
                //    DateOfBirth = request.DateOfBirth,
                //    Role = "User",
                //    Status = true,
                //    Image = ""
                //};

                var newUser = _mapper.Map<users>(request);
                newUser.Role = "User";
                newUser.PassWord = Utility.UtilityFunction.CreateMD5(request.PassWord);
                _userRepository.Create(newUser);
                _userRepository.SaveChange();
                return new MessageData { Data = newUser, Des = "Register successfull" };
            }
            catch(Exception ex)
            {
                return new MessageData { Data = null, Des = ex.ToString()};
            }
        }
    }
}
