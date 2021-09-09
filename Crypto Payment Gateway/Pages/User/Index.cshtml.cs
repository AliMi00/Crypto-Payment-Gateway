using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway.Models.DbModels;
using Crypto_Payment_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Crypto_Payment_Gateway.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly IAccountingServices accountingServices;
        public IndexModel(ILogger<IndexModel> logger, IAccountingServices accountingServices)
        {
            this.accountingServices = accountingServices;
            this.logger = logger;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var model = new SiteUser()
            {
                Address = "as"
            };
            return Page();
        }
        public void OnPost()
        {

        }
    }
}
