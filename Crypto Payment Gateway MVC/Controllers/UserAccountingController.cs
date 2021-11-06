﻿using Crypto_Payment_Gateway_MVC.Models.DbModels;
using Crypto_Payment_Gateway_MVC.Models.Enums;
using Crypto_Payment_Gateway_MVC.Models.InternalModels;
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
        //get user transactions
        public async Task<IActionResult> GetTransactions()
        {
            SiteUser siteUser = await userManager.GetUserAsync(User);
            IList<TransactionView> Transactions = accountingServices.GetUserTransactions(siteUser, null, null, 10).ToList();

            return View("Transactions",Transactions);
        }

        //show deposit page for amount and currency
        public async Task<IActionResult> GetDeposit()
        {
            SiteUser siteUser = await userManager.GetUserAsync(User);

            IList<WalletViewModel> wallets = accountingServices.GetAvailablecurrencyWallet().ToList();
            var models = wallets.GroupBy(x => x.Currency);
            return View("Deposit",models);
        }

        //request for deposit by user
        public async Task<IActionResult> GetRequestDeposit(Currency currency,float amount)
        {
            SiteUser siteUser = await userManager.GetUserAsync(User);

            AccountingIncreaseBalance increaseBalance = await accountingServices.UserIncreaseBalanceRequest(currency, amount, siteUser);

            if (increaseBalance.Error)
            {
                //change for showing error page
                return null;
            }
            else if (!increaseBalance.Error)
            {
                RequestDepositingViewModel model = new RequestDepositingViewModel()
                {
                    Amount = increaseBalance.Amount,
                    Error = increaseBalance.Error,
                    Message = increaseBalance.Message,
                    ReciveWallet = increaseBalance.ReciveWallet,
                    StartedTime = increaseBalance.StartedTime
                };
                return View("DepositingPay", model);
            }
            return null;
        }
        

    }
}
