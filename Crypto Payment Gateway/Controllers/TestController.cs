using Crypto_Payment_Gateway.Models.DbModels;
using Crypto_Payment_Gateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crypto_Payment_Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly IAccountingServices accountingServices;
        public TestController(IAccountingServices accountingServices)
        {
            this.accountingServices = accountingServices;
        }

        [Produces("application/json")]
        public async Task<IActionResult> GetTransactions()
        {

            ICollection<WalletTransaction> walletTransactions = await accountingServices.AddWalletTransactions(DateTime.Now, Models.Enums.Currency.USDTerc20);


            return Json(walletTransactions);
        }
    }
}
