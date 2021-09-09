using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Crypto_Payment_Gateway.Data;
using Crypto_Payment_Gateway.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Crypto_Payment_Gateway.Services;
using Microsoft.AspNetCore.Authorization;

namespace Crypto_Payment_Gateway.Pages.User
{
    [Authorize]
    public class TransactionsModel : PageModel
    {
        private readonly ILogger<TransactionsModel> logger;
        private readonly IAccountingServices accountingServices;
        private readonly UserManager<SiteUser> userManager;


        public TransactionsModel(ILogger<TransactionsModel> logger, IAccountingServices accountingServices, UserManager<SiteUser> userManager)
        {
            this.accountingServices = accountingServices;
            this.logger = logger;
            this.userManager = userManager;
        }


        public IList<Transactions> Transactions { get; set; }

        public async Task OnGetAsync()
        {
            SiteUser siteUser = await userManager.GetUserAsync(User);
            Transactions = accountingServices.GetUserTransactions(siteUser, null, null, 10).ToList();
        }
    }
}
