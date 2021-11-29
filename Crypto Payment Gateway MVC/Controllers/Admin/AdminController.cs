using Crypto_Payment_Gateway_MVC.Models.DbModels;
using Crypto_Payment_Gateway_MVC.Models.Enums;
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

namespace Crypto_Payment_Gateway_MVC.Controllers.Admin
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> logger;
        private readonly IAccountingServices accountingServices;
        private readonly IAdminGeneralServices adminGeneralServices;
        private readonly UserManager<SiteUser> userManager;

        public AdminController(ILogger<AdminController> logger, IAccountingServices accountingServices, UserManager<SiteUser> userManager,IAdminGeneralServices adminGeneralServices)
        {
            this.accountingServices = accountingServices;
            this.logger = logger;
            this.userManager = userManager;
            this.adminGeneralServices = adminGeneralServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddWallet()
        {
            return View("AddWallet");
        }

        [HttpPost]
        public IActionResult AddWallet(AddWalletViewModel walletViewModel)
        {
            GeneralResposeViewModel respose = new GeneralResposeViewModel();
            try
            {
                Wallet wallet = new Wallet()
                {
                    Currency = walletViewModel.Currency,
                    IsAvailable = walletViewModel.IsAvailable,
                    IsMainWallet = walletViewModel.IsMainWallet,
                    PrivateKey = walletViewModel.PrivateKey,
                    WalletAddress = walletViewModel.WalletAddress,
                    UsedCounter = walletViewModel.UsedCounter
                };
                adminGeneralServices.AddWallet(wallet);

                respose.Error = false;
                respose.Message = "Added";

            }
            catch (Exception e)
            {
                respose.Message = e.Message;
                respose.Error = true;
                
            }
            


            return View("AddWalletRespons",respose);
        }

        [HttpGet]
        public IActionResult GetWallet(Currency currency)
        {
            List<WalletViewModel> walletViewModel = adminGeneralServices
                .GetWallets(currency)
                .Select(x => new WalletViewModel() 
                    {
                        Currency = x.Currency,
                        Id = x.Id,
                        IsAvailable = x.IsAvailable,
                        WalletAddress = x.WalletAddress,    
                        ReleaseDate = x.ReleaseDate
                    }).ToList();

            return View("GetWallet",walletViewModel);
        }
    }
}
