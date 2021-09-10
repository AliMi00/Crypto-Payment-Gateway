using Crypto_Payment_Gateway_MVC.Models.DbModels;
using Crypto_Payment_Gateway_MVC.Models.ViewModels;
using Crypto_Payment_Gateway_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway_MVC.Controllers
{
    [Authorize]
    public class UserAccountingController : Controller
    {
        private readonly ILogger<UserAccountingController> logger;
        private readonly IAccountingServices accountingServices;
        private readonly UserManager<SiteUser> userManager;

        public UserAccountingController(ILogger<UserAccountingController> logger, IAccountingServices accountingServices, UserManager<SiteUser> userManager)
        {
            this.accountingServices = accountingServices;
            this.logger = logger;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetTransactions()
        {
            SiteUser siteUser = await userManager.GetUserAsync(User);
            IList<TransactionView> Transactions = accountingServices.GetUserTransactions(siteUser, null, null, 10).ToList();

            return View(Transactions);
        }

    }
}
