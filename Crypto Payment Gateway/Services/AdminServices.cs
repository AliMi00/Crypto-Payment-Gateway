using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway.Data;
using Crypto_Payment_Gateway.Models.DbModels;

namespace Crypto_Payment_Gateway.Services
{
    public class AdminServices: IAdminServices
    {
        private readonly IApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<AdminServices> logger;
        private readonly UserManager<SiteUser> UserManager;

        public AdminServices(IApplicationDbContext db, IWebHostEnvironment webHostEnvironment, ILogger<AdminServices> _logger, IServiceProvider serviceProvider)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            logger = _logger;
            UserManager = serviceProvider.GetRequiredService<UserManager<SiteUser>>();
        }

        public Task AddDataToFrontPage()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddUserToManager(SiteUser siteUser)
        {
            var result = await UserManager.AddToRoleAsync(siteUser, "Manager");
            return result.Succeeded;
        }

        public Task CreateCoupon()
        {
            throw new NotImplementedException();
        }

        public Task DisableCoupon()
        {
            throw new NotImplementedException();
        }

        public Task GetAllTicket()
        {
            throw new NotImplementedException();
        }

        public Task ManagingGames()
        {
            throw new NotImplementedException();
        }

        public Task ManagingreReferee()
        {
            throw new NotImplementedException();
        }
    }
}
