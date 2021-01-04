




using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ParkDbContext _parkDbContext;
        private readonly AppSettings _appSettings;
        
        public UserRepository(ParkDbContext parkDbContext, IOptions<AppSettings> appSettings)
        {
            _parkDbContext = parkDbContext;
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _parkDbContext.Users.SingleOrDefault(u => u.Username == username && u.Password == password);

            // User (usuário) não encontrado
            if (user == null)
            {
                return null;
            }

            // Usuário encontrado, criação do token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            //user.Password = "";
            
            return user;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _parkDbContext.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        public User Register(string username, string password)
        {
            User userObj = new User()
            {
                Username = username,
                Password = password,
                Role = "Admin"
            };

            _parkDbContext.Users.Add(userObj);
            _parkDbContext.SaveChanges();
            userObj.Password = "";

            return userObj;
        }
    }
}
