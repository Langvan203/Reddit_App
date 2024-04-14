using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Reddit_App.Common;
using Reddit_App.Database;
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
                if (user.UserName == "admin" && user.PassWord == "admin")
                {
                    var newClaimList = claimList.ToList();
                    //newClaimList.RemoveAll(claim => claim.Type == ClaimTypes.Role && claim.Value == "User");
                    newClaimList.Add(new Claim(ClaimTypes.Role, "Admin"));
                    // renew claim
                    claimList = newClaimList.ToArray();
                }
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
                    user = user
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object UserRegister(UserRegisterRequest request)
        {
            try
            {
                var user = _userRepository.FindByCondition(r => r.UserName == request.UserName).FirstOrDefault();
                if(user != null)
                {
                    throw new Exception("Username has been used");
                }
                var newUser = new users()
                {
                    UserName = request.UserName,
                    PassWord = request.PassWord,
                    Email = request.Email,
                    DateOfBirth = request.DateOfBirth
                };
                _userRepository.Create(newUser);
                _userRepository.SaveChange();

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiOption.Secret));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                var claimList = new[]
                {
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.UserData, newUser.UserName),
                    new Claim(ClaimTypes.Sid, newUser.UserID.ToString()),
                };

                var token = new JwtSecurityToken(
                    issuer: _apiOption.ValidIssuer,
                    audience: _apiOption.ValidAudience,
                    expires: DateTime.Now.AddDays(1),
                    claims: claimList,
                    signingCredentials: credentials
                    );

                var tokenByString = new JwtSecurityTokenHandler().WriteToken(token);
                return new {
                    token = tokenByString,
                    user = newUser
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
