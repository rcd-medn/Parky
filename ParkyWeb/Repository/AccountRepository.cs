




using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class AccountRepository : Repository<User>, IAccountRepository
    {
        private readonly IHttpClientFactory _httpClient;

        public AccountRepository(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<User> LoginAsync(string url, User objToCreate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterAsync(string url, User objToRegister)
        {
            throw new NotImplementedException();
        }
    }
}
