﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public class InMemoryTicketStore : ITicketStore
    {
        private readonly IMemoryCache _cache;

        public InMemoryTicketStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);

            return Task.CompletedTask;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _cache.Set(key, ticket);

            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var ticket = _cache.Get<AuthenticationTicket>(key);

            return Task.FromResult(ticket);
        }

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = ticket.Principal.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            _cache.Set(key, ticket);

            return Task.FromResult(key);
        }
    }
}
