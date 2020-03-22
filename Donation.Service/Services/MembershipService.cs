using AutoMapper;
using Donation.Common.ViewModels;
using Donation.Data.Repository;
using Donation.Domain.Collections;
using Donation.Domain.Dtos;
using Donation.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Donation.Common.Enums;

namespace Donation.Service.Services
{
    public class MembershipService : IMembershipServices
    {
        private readonly IMongoDBRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _iconfiguration;

        public MembershipService(IMongoDBRepository<User> userRepository, IMapper mapper, IConfiguration iconfiguration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _iconfiguration = iconfiguration;
        }

        public async Task<ResponseViewModel> RegisterUser(UserDto model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {

                var user = _mapper.Map<User>(model);
                var existingUser = GetSingleByUsernameorEmail(user.Email);
                if (!existingUser)
                {
                    if (model.Password != null)
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);
                        user.HashedPassword = passwordHash; user.Salt = passwordSalt; user.IsActive = true;
                        var result = await _userRepository.Add(user);
                        if (result != null)
                        {
                            response.Message = Constants.Register;
                            response.ResponseData = model;
                            response.Status = (int)Number.One;
                            await Task.Run(async () =>
                            {
                                //var content = _generateEmail.GenerateRegisterEmailBody(_appSettings.Value.ClientApplicationUrl, result.Id);
                                //await _email.SendEmail(Constants.Register, content, user.Email);
                            });


                        }
                        else
                        {
                            response.Message = Constants.Error;
                            response.Status = (int)Number.Zero;
                        }
                    }
                    else
                    {
                        response.Message = Constants.PasswordRequired;
                        response.Status = (int)Number.Zero;
                    }
                }
                else
                {
                    response.Message = Constants.UserExist;
                    response.Status = (int)Number.One;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public ResponseViewModel Login(string email, string password)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                UserDto model = new UserDto();
                model.Email = email;
                model.Password = password;
                var user = _mapper.Map<User>(model);
                Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(user.Email)) && x.Email.ToLower() == user.Email.ToLower());
                var userObj = _userRepository.GetById(whereCondition).AsQueryable().FirstOrDefault();
                if (userObj == null)
                    response.Message = Constants.InvalidAccount;
                else
                {
                    if (userObj.IsActive == true)
                    {
                        if (!VerifyPasswordHash(model.Password, userObj.HashedPassword, userObj.Salt))
                            response.Message = Constants.InvalidPassword;
                        else
                        {
                            // var result = _mapper.Map<UserDto>(userObj);
                            var userNew = BuildToken(userObj);
                            response.Message = Constants.Retreived;
                            response.Status = (int)Number.One;
                            response.ResponseData = new { Token = userNew.Token, Email = userNew.Email };
                        }
                    }
                    else
                    {
                        response.Message = Constants.NotActive;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private User BuildToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_iconfiguration.GetSection("AppSettings").GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }

        public bool GetSingleByUsernameorEmail(string email)
        {
            Expression<Func<User, bool>> whereCondition = x => ((!string.IsNullOrEmpty(email) && x.Email.ToLower() == email.ToLower()));
            return _userRepository.Exist(whereCondition);
        }

        #region PasswordEncryption
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}
