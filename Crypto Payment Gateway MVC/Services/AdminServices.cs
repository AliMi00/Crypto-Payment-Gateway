using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crypto_Payment_Gateway_MVC.Data;
using Crypto_Payment_Gateway_MVC.Models.DbModels;
using Crypto_Payment_Gateway_MVC.Models.Enums;
using Crypto_Payment_Gateway_MVC.Models.InternalModels;

namespace Crypto_Payment_Gateway_MVC.Services
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
        //TODO: probebly need modififactions
        public ICollection<Transactions> GetAllTransactions(DateTime startDate, DateTime endDate)
        {
            ICollection<Transactions> transactions = db.Transactions.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            return transactions;
        }

        public ICollection<Wallet> GetWallets(Currency currency = Currency.Other)
        {
            ICollection<Wallet> wallets = db.Wallets.Where(x => x.Currency == currency).ToList();
            return wallets;
        }

        public ICollection<WalletTransaction> GetWalletTransactions(DateTime startDate, DateTime endDate)
        {
            ICollection<WalletTransaction> walletTransactions = db.WalletTransactions.Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();

            return walletTransactions;
        }

        public ICollection<Withdraw> GetWithdrawsRequest(WithdrawStatus status)
        {
            ICollection<Withdraw> withdraws = db.Withdraws.Where(x => x.Status == status).ToList();

            return withdraws;
        }

        //TODO: need to modify by viewModels
        public async Task<GeneralResponse> AddWallet(Wallet wallet)
        {
            GeneralResponse response = new(); 
            if(db.Wallets.Any(x => x.WalletAddress == wallet.WalletAddress))
            {
                response.Status = Status.Faild;
                response.Message = "Existing Address";

                return response;
            }
            else
            {
                await db.Wallets.AddAsync(wallet);
                await db.SaveChangesAsync(true);

                response.Status = Status.SuccessFul;
                response.Message = "Added";
                return response;
            }
        }

        //TODO: need to modify by viewModels
        public GeneralResponse ModifyWithdrawsResquest(Withdraw withdraw)
        {
             db.Withdraws.Update(withdraw);
            db.SaveChanges();

            return new GeneralResponse()
            {
                Message = "Ok",
                Status = Status.SuccessFul
            };


        }

        public GeneralResponse ModifyWallet(Wallet wallet)
        {
            db.Wallets.Update(wallet);
            db.SaveChanges();

            return new GeneralResponse()
            {
                Message = "Ok",
                Status = Status.SuccessFul
            };
        }
    }
}
